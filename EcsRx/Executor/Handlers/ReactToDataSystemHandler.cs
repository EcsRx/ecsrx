using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(4)]
    public class ReactToDataSystemHandler : IConventionalSystemHandler
    {
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IDictionary<ISystem, IDictionary<Guid, IDisposable>> _entitySubscriptions;
        public readonly IPoolManager _poolManager;

        private readonly MethodInfo _processEntityMethod;
        
        public ReactToDataSystemHandler(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<Guid, IDisposable>>();
            _processEntityMethod = GetType().GetMethod("ProcessEntity");
        }

        // TODO: This is REALLY bad but currently no other way around the dynamic invocation lookup stuff
        public Func<IEntity, IDisposable> CreateEntityProcessorFunction(ISystem system)
        {
            var genericDataType = system.GetGenericDataType();
            var genericMethod = _processEntityMethod.MakeGenericMethod(genericDataType);
            return entity => (IDisposable) genericMethod.Invoke(this, new object[] {system, entity});
        }

        public IDisposable ProcessEntity<T>(IReactToDataSystem<T> system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var reactObservable = system.ReactToData(entity);
            
            if (!hasEntityPredicate)
            { return reactObservable.Subscribe(x => system.Execute(entity, x)); }
            
            var groupPredicate = system.TargetGroup as IHasPredicate;
            return reactObservable
                .Where(x => groupPredicate.CanProcessEntity(entity))
                .Subscribe(x => system.Execute(entity, x));
        }

        public bool CanHandleSystem(ISystem system)
        { return system.IsReactiveDataSystem(); }

        public void SetupSystem(ISystem system)
        {
            var processEntityFunction = CreateEntityProcessorFunction(system);

            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);
            
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            _entitySubscriptions.Add(system, entitySubscriptions);
            
            var groupAccessor = _poolManager.CreateObservableGroup(system.TargetGroup);

            groupAccessor.OnEntityAdded
                .Subscribe(x =>
                {
                    var subscription = processEntityFunction(x);
                    entitySubscriptions.Add(x.Id, subscription);
                })
                .AddTo(entityChangeSubscriptions);
            
            groupAccessor.OnEntityRemoved
                .Subscribe(x =>
                {
                    entitySubscriptions.RemoveAndDispose(x.Id);
                })
                .AddTo(entityChangeSubscriptions);
            
            
            foreach (var entity in groupAccessor.Entities)
            {
                var subscription = processEntityFunction(entity);
                entitySubscriptions.Add(entity.Id, subscription);
            }
        }

        public void DestroySystem(ISystem system)
        {
            _systemSubscriptions.RemoveAndDispose(system);
            
            var entitySubscriptions = _entitySubscriptions[system];
            entitySubscriptions.Values.DisposeAll();
            entitySubscriptions.Clear();
            _entitySubscriptions.Remove(system);
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