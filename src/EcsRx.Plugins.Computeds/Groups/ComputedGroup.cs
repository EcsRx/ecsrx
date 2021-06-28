using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Lookups;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Plugins.Computeds.Groups
{
    public abstract class ComputedGroup : IObservableGroup, IDisposable
    {
        public readonly EntityLookup CachedEntities;
        public readonly IList<IDisposable> Subscriptions;
        
        public ObservableGroupToken Token => InternalObservableGroup.Token;
        public IObservable<IEntity> OnEntityAdded => _onEntityAdded;
        public IObservable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public IObservable<IEntity> OnEntityRemoving => _onEntityRemoving;

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        public IObservableGroup InternalObservableGroup { get; }

        protected ComputedGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            CachedEntities = new EntityLookup();
            Subscriptions = new List<IDisposable>();
            
            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            MonitorChanges();
            RefreshEntities();
        }

        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(OnEntityAddedToGroup).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(OnEntityRemovingFromGroup).AddTo(Subscriptions);
            RefreshWhen().Subscribe(x => RefreshEntities()).AddTo(Subscriptions);
        }

        public void OnEntityAddedToGroup(IEntity entity)
        {
            if (!IsEntityApplicable(entity))
            { return; }
            
            CachedEntities.Add(entity);
            _onEntityAdded.OnNext(entity);
        }
        
        public void OnEntityRemovingFromGroup(IEntity entity)
        {
            if(!CachedEntities.Contains(entity.Id))
            { return; }
            
            _onEntityRemoving.OnNext(entity);
            CachedEntities.Remove(entity.Id);
            _onEntityRemoved.OnNext(entity);
        }

        public void RefreshEntities()
        {
            var applicableEntities = InternalObservableGroup.Where(IsEntityApplicable).ToArray();
            var entitiesToRemove = InternalObservableGroup.Where(x => applicableEntities.All(y => y.Id != x.Id)).ToArray();
            var entitiesToAdd = applicableEntities.Where(x => !CachedEntities.Contains(x.Id)).ToArray();
            
            for (var i = entitiesToAdd.Length - 1; i >= 0; i--)
            {
                CachedEntities.Add(entitiesToAdd[i]);
                _onEntityAdded.OnNext(entitiesToAdd[i]);
            }

            for (var i = entitiesToRemove.Length - 1; i >= 0; i--)
            {
                _onEntityRemoving.OnNext(entitiesToRemove[i]);
                CachedEntities.Remove(entitiesToRemove[i].Id);
                _onEntityRemoved.OnNext(entitiesToRemove[i]);
            }
        }
        
        public bool ContainsEntity(int id)
        { return CachedEntities.Contains(id); }

        public IEntity GetEntity(int id)
        { return CachedEntities[id]; }

        /// <summary>
        /// The method to indicate when the listings should be updated
        /// </summary>
        /// <remarks>
        /// If there is no checking required outside of adding/removing this can
        /// return an empty observable, but common usages would be to refresh every update.
        /// The bool is throw away, but is a workaround for not having a Unit class
        /// </remarks>
        /// <returns>An observable trigger that should trigger when the group should refresh</returns>
        public abstract IObservable<bool> RefreshWhen();
        
        /// <summary>
        /// The method to check if the entity is applicable to this computed group
        /// </summary>
        /// <param name="entity">The entity to check on</param>
        /// <returns>true if it should be in the group, false if it should not</returns>
        public abstract bool IsEntityApplicable(IEntity entity);

        public virtual IEnumerable<IEntity> PostProcess(IEnumerable<IEntity> entities)
        { return entities; }

        public virtual IEnumerator<IEntity> GetEnumerator()
        { return PostProcess(CachedEntities).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
        
        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityRemoving.Dispose();
        }

        public int Count => CachedEntities.Count;

        public IEntity this[int index] => CachedEntities.GetByIndex(index);
    }
}