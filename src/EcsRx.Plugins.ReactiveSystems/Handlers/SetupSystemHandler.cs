using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Handlers
{
    /*
    [Priority(10)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public IEntityCollectionManager EntityCollectionManager { get; }
        
        private Dictionary<int, List<ISetupSystem>> _groupSystems = new Dictionary<int, List<ISetupSystem>>();
        private Dictionary<int, IDisposable> _groupDisposables = new Dictionary<int, IDisposable>();
        private IDictionary<ISetupSystem, IDictionary<int, IDisposable>> _systemDisposables = new Dictionary<ISetupSystem, IDictionary<int, IDisposable>>();

        public SetupSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (ISetupSystem) system;
            var affinities = system.GetGroupAffinities();
            var observableGroup = EntityCollectionManager.GetObservableGroup(system.Group, affinities);
            var hashcode = observableGroup.Token.GetHashCode();
            
            if (_groupSystems.ContainsKey(hashcode))
            {
                _groupSystems[hashcode].Add(castSystem);
                return;
            }
            
            _groupSystems.Add(hashcode, new List<ISetupSystem> { castSystem });

            var combinedSub = new CompositeDisposable();

            observableGroup.OnEntityAdded
                .Subscribe(x => ProcessEntity(_groupSystems[hashcode], x))
                .AddTo(combinedSub);
            
            observableGroup.OnEntityRemoved
                .Subscribe(RemoveEntitySubs)
                .AddTo(combinedSub);
            
            _groupDisposables.Add(hashcode, combinedSub);
        }

        public void ProcessEntity(IReadOnlyList<ISetupSystem> systems, IEntity entity)
        {
            for (var i = 0; i < systems.Count; i++)
            {
                var currentSystem = systems[i];
                var possibleSub = ProcessEntity(currentSystem, entity);
                if (possibleSub == null) continue;
                
                if(!_systemDisposables.ContainsKey(currentSystem))
                {_systemDisposables.Add(currentSystem, new Dictionary<int, IDisposable>()); }
                    
                _systemDisposables[currentSystem].Add(entity.Id, possibleSub);
            }
        }
        
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
                    _systemDisposables[system].Remove(x.Result.Id);
                    system.Setup(x.Result);
                });

            return disposable;
        }

        public void RemoveEntitySubs(IEntity entity)
        {
            foreach (var systemLookup in _systemDisposables)
            {
                if (systemLookup.Value.ContainsKey(entity.Id))
                { systemLookup.Value.RemoveAndDispose(entity.Id); }
            }
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = system as ISetupSystem;
            foreach (var tokenLookups in _groupSystems)
            {
                if (tokenLookups.Value.Contains(castSystem))
                { tokenLookups.Value.Remove(castSystem); }
            }
        }

        public void Dispose()
        {
            _groupSystems.Clear();
            _groupDisposables.DisposeAll();
            _systemDisposables.ForEachRun(x => x.Value.RemoveAndDisposeAll());
            _systemDisposables.Clear();
        }
    }
    */
    /*
    [Priority(10)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public IEntityCollectionManager EntityCollectionManager { get; }
        
        private Dictionary<ObservableGroupToken, List<ISetupSystem>> _groupSystems = new Dictionary<ObservableGroupToken, List<ISetupSystem>>();
        private Dictionary<ObservableGroupToken, IDisposable> _groupDisposables = new Dictionary<ObservableGroupToken, IDisposable>();
        private IDictionary<ISetupSystem, IDictionary<int, IDisposable>> _systemDisposables = new Dictionary<ISetupSystem, IDictionary<int, IDisposable>>();

        public SetupSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (ISetupSystem) system;
            var affinities = system.GetGroupAffinities();
            var observableGroup = EntityCollectionManager.GetObservableGroup(system.Group, affinities);

            if (_groupSystems.ContainsKey(observableGroup.Token))
            {
                _groupSystems[observableGroup.Token].Add(castSystem);
                return;
            }
            
            _groupSystems.Add(observableGroup.Token, new List<ISetupSystem> { castSystem });

            var combinedSub = new CompositeDisposable();

            observableGroup.OnEntityAdded
                .Subscribe(x => ProcessEntity(_groupSystems[observableGroup.Token], x))
                .AddTo(combinedSub);
            
            observableGroup.OnEntityRemoved
                .Subscribe(RemoveEntitySubs)
                .AddTo(combinedSub);
            
            _groupDisposables.Add(observableGroup.Token, combinedSub);
        }

        public void ProcessEntity(IReadOnlyList<ISetupSystem> systems, IEntity entity)
        {
            for (var i = 0; i < systems.Count; i++)
            {
                var currentSystem = systems[i];
                var possibleSub = ProcessEntity(currentSystem, entity);
                if (possibleSub == null) continue;
                
                if(!_systemDisposables.ContainsKey(currentSystem))
                {_systemDisposables.Add(currentSystem, new Dictionary<int, IDisposable>()); }
                    
                _systemDisposables[currentSystem].Add(entity.Id, possibleSub);
            }
        }
        
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
                    _systemDisposables[system].Remove(x.Result.Id);
                    system.Setup(x.Result);
                });

            return disposable;
        }

        public void RemoveEntitySubs(IEntity entity)
        {
            foreach (var systemLookup in _systemDisposables)
            {
                if (systemLookup.Value.ContainsKey(entity.Id))
                { systemLookup.Value.RemoveAndDispose(entity.Id); }
            }
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = system as ISetupSystem;
            foreach (var tokenLookups in _groupSystems)
            {
                if (tokenLookups.Value.Contains(castSystem))
                { tokenLookups.Value.Remove(castSystem); }
            }
        }

        public void Dispose()
        {
            _groupSystems.Clear();
            _groupDisposables.DisposeAll();
            _systemDisposables.ForEachRun(x => x.Value.RemoveAndDisposeAll());
            _systemDisposables.Clear();
        }
    }*/
    
    
    [Priority(10)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityCollectionManager EntityCollectionManager;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public SetupSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
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
            var affinities = system.GetGroupAffinities();
            var observableGroup = EntityCollectionManager.GetObservableGroup(system.Group, affinities);

            observableGroup.OnEntityAdded
                .Subscribe(x =>
                {
                    var possibleSubscription = ProcessEntity(castSystem, x);
                    if(possibleSubscription != null) 
                    { entitySubscriptions.Add(x.Id, possibleSubscription); }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnEntityRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x.Id))
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);


            foreach (var entity in observableGroup)
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