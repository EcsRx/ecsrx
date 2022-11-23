using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Handlers
{
    [Priority(3)]
    public class ReactToDataSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager ObservableGroupManager;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        public readonly IDictionary<ISystem, IDictionary<int, IDisposable>> EntitySubscriptions;

        private readonly MethodInfo _processEntityMethod;
        
        public ReactToDataSystemHandler(IObservableGroupManager observableGroupManager)
        {
            ObservableGroupManager = observableGroupManager;
            SystemSubscriptions = new Dictionary<ISystem, IDisposable>();
            EntitySubscriptions = new Dictionary<ISystem, IDictionary<int, IDisposable>>();
            _processEntityMethod = GetType().GetMethod("ProcessEntity");
        }

        // TODO: This is REALLY bad but currently no other way around the dynamic invocation lookup stuff
        public IEnumerable<Func<IEntity, IDisposable>> CreateEntityProcessorFunctions(ISystem system)
        {
            var genericMethods = system.GetGenericDataTypes(typeof(IReactToDataSystem<>))
                .Select(x => _processEntityMethod.MakeGenericMethod(x));

            foreach (var genericMethod in genericMethods)
            { yield return entity => (IDisposable) genericMethod.Invoke(this, new object[] {system, entity}); }
        }
        
        public IDisposable ProcessEntity<T>(IReactToDataSystem<T> system, IEntity entity)
        {
            var hasEntityPredicate = system.Group is IHasPredicate;
            var reactObservable = system.ReactToData(entity);
            
            if (!hasEntityPredicate)
            { return reactObservable.Subscribe(x => system.Process(entity, x)); }
            
            var groupPredicate = system.Group as IHasPredicate;
            return reactObservable
                .Subscribe(x =>
                {
                    if(groupPredicate.CanProcessEntity(entity))
                    { system.Process(entity, x);}
                });
        }
        
        public bool CanHandleSystem(ISystem system)
        { return system.IsReactiveDataSystem(); }

        public void SetupSystem(ISystem system)
        {
            var processEntityFunctions = CreateEntityProcessorFunctions(system);

            var entityChangeSubscriptions = new CompositeDisposable();
            SystemSubscriptions.Add(system, entityChangeSubscriptions);
            var entitySubscriptions = new Dictionary<int, IDisposable>();
            EntitySubscriptions.Add(system, entitySubscriptions);

            var groupSystem = system as IGroupSystem;
            var affinities = groupSystem.GetGroupAffinities();
            var observableGroup = ObservableGroupManager.GetObservableGroup(groupSystem.Group, affinities);

            observableGroup.OnEntityAdded
                .Subscribe(x =>
                {
                    // This occurs if we have an add elsewhere removing the entity before this one is called
                    if (!observableGroup.ContainsEntity(x.Id)) { return; }
                    
                    var entityDisposables = new CompositeDisposable();
                    entitySubscriptions.Add(x.Id, entityDisposables);
                    foreach (var processFunction in processEntityFunctions)
                    {
                        var subscription = processFunction(x);
                        entityDisposables.Add(subscription);
                    }
                })
                .AddTo(entityChangeSubscriptions);
            
            observableGroup.OnEntityRemoved
                .Subscribe(x =>
                {
                    if (entitySubscriptions.ContainsKey(x.Id)) 
                    { entitySubscriptions.RemoveAndDispose(x.Id); }
                })
                .AddTo(entityChangeSubscriptions);

            var entitiesToProcess = observableGroup.ToArray();
            foreach (var entity in entitiesToProcess)
            {
                var entityDisposables = new CompositeDisposable();
                entitySubscriptions.Add(entity.Id, entityDisposables);
                
                foreach (var processFunction in processEntityFunctions)
                {
                    var subscription = processFunction(entity);
                    entityDisposables.Add(subscription);
                }
            }
        }

        public void DestroySystem(ISystem system)
        {
            SystemSubscriptions.RemoveAndDispose(system);
            
            var entitySubscriptions = EntitySubscriptions[system];
            entitySubscriptions.Values.DisposeAll();
            entitySubscriptions.Clear();
            EntitySubscriptions.Remove(system);
        }
        
        public void Dispose()
        {
            SystemSubscriptions.DisposeAll();
            foreach (var entitySubscriptions in EntitySubscriptions.Values)
            {
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
            }
            EntitySubscriptions.Clear();
        }
    }
}