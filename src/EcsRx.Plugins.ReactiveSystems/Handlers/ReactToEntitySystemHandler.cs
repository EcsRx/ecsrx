using System;
using System.Collections.Generic;
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
    [Priority(3)]
    public class ReactToEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IObservableGroupManager ObservableGroupManager;
        
        public ReactToEntitySystemHandler(IObservableGroupManager observableGroupManager)
        {
            ObservableGroupManager = observableGroupManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToEntitySystem; }
        
        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToEntitySystem) system;
            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = ObservableGroupManager.GetObservableGroup(castSystem.Group, affinities);            
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);
           
            observableGroup.OnEntityAdded
                .Subscribe(x =>
                {
                    var entitySubscription = ProcessEntity(castSystem, x);
                    entitySubscriptions.Add(x.Id, entitySubscription);
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnEntityRemoved
                .Subscribe(x =>
                {
                    entitySubscriptions.RemoveAndDispose(x.Id);
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in observableGroup)
            {
                var subscription = ProcessEntity(castSystem, entity);
                entitySubscriptions.Add(entity.Id, subscription);
            }
            
            _entitySubscriptions.Add(system, entitySubscriptions);
        }

        public void DestroySystem(ISystem system)
        {
            _systemSubscriptions.RemoveAndDispose(system);
            
            var entitySubscriptions = _entitySubscriptions[system];
            entitySubscriptions.Values.DisposeAll();
            entitySubscriptions.Clear();
            _entitySubscriptions.Remove(system);
        }
        
        public IDisposable ProcessEntity(IReactToEntitySystem system, IEntity entity)
        {
            var hasEntityPredicate = system.Group is IHasPredicate;
            var reactObservable = system.ReactToEntity(entity);
            
            if (!hasEntityPredicate)
            { return reactObservable.Subscribe(system.Process); }

            var groupPredicate = system.Group as IHasPredicate;
            return reactObservable
                .Subscribe(x =>
                {
                    if(groupPredicate.CanProcessEntity(x))
                    { system.Process(x); }
                });
        }

        public void Dispose()
        {
            _systemSubscriptions.DisposeAll();
            foreach (var entitySubscriptions in _entitySubscriptions.Values)
            {
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
            }
            _entitySubscriptions.Clear();
        }
    }
}