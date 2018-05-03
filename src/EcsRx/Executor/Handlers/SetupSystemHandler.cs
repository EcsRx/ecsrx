using System;
using System.Collections.Generic;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Polyfills;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(1)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityCollectionManager EntityCollectionManager;
        public readonly IDictionary<ISystem, IDictionary<Guid, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public SetupSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<Guid, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            _entitySubscriptions.Add(system, entitySubscriptions);
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ISetupSystem) system;
            var accessor = EntityCollectionManager.CreateObservableGroup(system.TargetGroup);

            accessor.OnEntityAdded
                .Subscribe(x =>
                {
                    var possibleSubscription = ProcessEntity(castSystem, x);
                    if(possibleSubscription != null) 
                    { entitySubscriptions.Add(x.Id, possibleSubscription); }
                })
                .AddTo(entityChangeSubscriptions);
            
            accessor.OnEntityRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in accessor.Entities)
            {
                var possibleSubscription = ProcessEntity(castSystem, entity);
                if (possibleSubscription != null)
                { entitySubscriptions.Add(entity.Id, possibleSubscription); }
            }         
        }

        public void DestroySystem(ISystem system)
        {
            _systemSubscriptions.RemoveAndDispose(system);
        }

        public IDisposable ProcessEntity(ISetupSystem system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;

            if (!hasEntityPredicate)
            {
                system.Setup(entity);
                return null;
            }

            var groupPredicate = system.TargetGroup as IHasPredicate;

            if (groupPredicate.CanProcessEntity(entity))
            {
                system.Setup(entity);
                return null;
            }

            entity.WaitForPredicateMet(groupPredicate.CanProcessEntity)
                .ContinueWith(x =>
                {
                    _entitySubscriptions[system].Remove(x.Result.Id);
                    system.Setup(x.Result);
                });

            return Disposable.Empty;
        }

        public void Dispose()
        {
            _systemSubscriptions.DisposeAll();
        }
    }
}