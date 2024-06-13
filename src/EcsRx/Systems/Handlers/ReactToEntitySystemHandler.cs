using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using R3;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Systems;

namespace EcsRx.Systems.Handlers
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
            _entitySubscriptions.Add(system, entitySubscriptions);

            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);
           
            observableGroup.OnEntityAdded
                .Subscribe(x =>
                {
                    // This occurs if we have an add elsewhere removing the entity before this one is called
                    if (observableGroup.ContainsEntity(x.Id))
                    { SetupEntity(castSystem, x, entitySubscriptions); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnEntityRemoved
                .Subscribe(x =>
                {
                    // This is if the add elsewhere removes the entity, which triggers this before the add is
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            var entities = observableGroup.ToArray();
            foreach (var entity in entities)
            { SetupEntity(castSystem, entity, entitySubscriptions); }
        }

        public void SetupEntity(IReactToEntitySystem system, IEntity entity, Dictionary<int, IDisposable> subs)
        {
            subs.Add(entity.Id, null);
                
            var subscription = ProcessEntity(system, entity);
            
            if (subs.ContainsKey(entity.Id))
            { subs[entity.Id] = subscription; }
            else
            { subscription.Dispose(); }
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