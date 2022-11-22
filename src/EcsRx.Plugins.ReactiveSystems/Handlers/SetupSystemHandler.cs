using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Handlers
{
    [Priority(10)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager ObservableGroupManager;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public SetupSystemHandler(IObservableGroupManager observableGroupManager)
        {
            ObservableGroupManager = observableGroupManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            _entitySubscriptions.Add(system, entitySubscriptions);
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ISetupSystem) system;
            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = ObservableGroupManager.GetObservableGroup(castSystem.Group, affinities);

            observableGroup.OnEntityAdded
                .Subscribe(x =>
                {
                    var entityDisposables = new CompositeDisposable();
                    entitySubscriptions.Add(x.Id, entityDisposables);
                    
                    var possibleSubscription = ProcessEntity(castSystem, x);
                    if(possibleSubscription != null) 
                    { entityDisposables.Add(possibleSubscription); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnEntityRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            var entities = observableGroup.ToArray();
            foreach (var entity in entities)
            {
                var entityDisposables = new CompositeDisposable();
                entitySubscriptions.Add(entity.Id, entityDisposables);
                
                var possibleSubscription = ProcessEntity(castSystem, entity);
                if (possibleSubscription != null)
                { entityDisposables.Add(possibleSubscription); }
            }
        }

        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public IDisposable ProcessEntity(ISetupSystem system, IEntity entity)
        {
            var hasEntityPredicate = system.Group is IHasPredicate;

            if (!hasEntityPredicate)
            {
                system.Setup(entity);
                return null;
            }

            var groupPredicate = system.Group as IHasPredicate;

            if (groupPredicate.CanProcessEntity(entity))
            {
                system.Setup(entity);
                return null;
            }

            var disposable = entity
                .WaitForPredicateMet(groupPredicate.CanProcessEntity)
                .ContinueWith(x =>
                {
                    _entitySubscriptions[system].Remove(x.Result.Id);
                    system.Setup(x.Result);
                });

            return disposable;
        }

        public void Dispose()
        {
            _systemSubscriptions.DisposeAll();
        }
    }
}