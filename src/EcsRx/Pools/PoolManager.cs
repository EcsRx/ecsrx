using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Pools
{
    public class PoolManager : IPoolManager, IDisposable
    {
        public const string DefaultPoolName = "default";
        
        private readonly IDictionary<ObservableGroupToken, IObservableGroup> _groupAccessors;
        private readonly IDictionary<string, IPool> _pools;

        public IEventSystem EventSystem { get; }
        public IEnumerable<IPool> Pools => _pools.Values;
        public IPoolFactory PoolFactory { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }

        public PoolManager(IEventSystem eventSystem, IPoolFactory poolFactory, IObservableGroupFactory observableGroupFactory)
        {
            EventSystem = eventSystem;
            PoolFactory = poolFactory;
            ObservableGroupFactory = observableGroupFactory;

            _groupAccessors = new Dictionary<ObservableGroupToken, IObservableGroup>();
            _pools = new Dictionary<string, IPool>();

            CreatePool(DefaultPoolName);
        }
        
        public IPool CreatePool(string name)
        {
            var pool = PoolFactory.Create(name);
            _pools.Add(name, pool);

            EventSystem.Publish(new PoolAddedEvent(pool));

            return pool;
        }

        public IPool GetPool(string name = null)
        { return _pools[name ?? DefaultPoolName]; }

        public void RemovePool(string name)
        {
            if(!_pools.ContainsKey(name)) { return; }

            var pool = _pools[name];
            _pools.Remove(name);

            EventSystem.Publish(new PoolRemovedEvent(pool));
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(IGroup group, string poolName = null)
        {
            if(group is EmptyGroup)
            { return new IEntity[0]; }

            if (poolName != null)
            { return _pools[poolName].Entities.MatchingGroup(group); }

            return Pools.GetAllEntities().MatchingGroup(group);
        }

        public IObservableGroup CreateObservableGroup(IGroup group, string poolName = null)
        {
            var groupAccessorToken = new ObservableGroupToken(group.MatchesComponents.ToArray(), poolName);
            if (_groupAccessors.ContainsKey(groupAccessorToken)) { return _groupAccessors[groupAccessorToken]; }

            var entityMatches = GetEntitiesFor(group, poolName);
            var groupAccessor = ObservableGroupFactory.Create(new ObservableGroupConfiguration
            {
                ObservableGroupToken = groupAccessorToken,
                InitialEntities = entityMatches
            });
            
            _groupAccessors.Add(groupAccessorToken, groupAccessor);

            return _groupAccessors[groupAccessorToken];
        }

        public void Dispose()
        {
            foreach (var accessor in _groupAccessors.Values)
            { (accessor as IDisposable)?.Dispose(); }
        }
    }
}