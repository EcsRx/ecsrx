using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;

namespace EcsRx.Computed
{
    public abstract class ComputedCollectionFromGroup<T> : IComputedCollection<T>, IDisposable
    {
        public IDictionary<int, T> FilteredCache { get; }
        public List<IDisposable> Subscriptions { get; }
        
        public IObservable<CollectionElementChangedEvent<T>> OnAdded => _onElementAdded;
        public IObservable<CollectionElementChangedEvent<T>> OnRemoved => _onElementChanged;
        public IObservable<CollectionElementChangedEvent<T>> OnUpdated => _onElementChanged;

        public IObservableGroup InternalObservableGroup { get; }
        public IEnumerable<T> Value => GetData();
        public T this[int index] => FilteredCache[index];
        public int Count => FilteredCache.Count;
        
        private readonly Subject<IEnumerable<T>> _onDataChanged;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementAdded;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementChanged;
        private readonly Subject<CollectionElementChangedEvent<T>> _onElementRemoved;
        private bool _needsUpdate;

        public ComputedCollectionFromGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();       
            FilteredCache = new Dictionary<int, T>();
            
            _onDataChanged = new Subject<IEnumerable<T>>();
            _onElementAdded = new Subject<CollectionElementChangedEvent<T>>();
            _onElementChanged = new Subject<CollectionElementChangedEvent<T>>();
            _onElementRemoved = new Subject<CollectionElementChangedEvent<T>>();
            
            MonitorChanges();
            RefreshData();
        }
        
        
        public IDisposable Subscribe(IObserver<IEnumerable<T>> observer)
        { return _onDataChanged.Subscribe(observer); }
        
        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(RequestUpdate).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(RequestUpdate).AddTo(Subscriptions);
            RefreshWhen().Subscribe(x => RequestUpdate()).AddTo(Subscriptions);
        }

        public void RequestUpdate(object _ = null)
        {
            _needsUpdate = true;
            
            if(_onDataChanged.HasObservers || _onElementAdded.HasObservers || _onElementChanged.HasObservers || _onElementRemoved.HasObservers)
            { RefreshData(); }
        }
        
        public void RefreshData()
        {
            foreach (var entity in InternalObservableGroup)
            {
                var isApplicable = ShouldTransform(entity);

                if (!isApplicable)
                {
                    if (FilteredCache.ContainsKey(entity.Id))
                    {
                        var currentValue = FilteredCache[entity.Id];
                        FilteredCache.Remove(entity.Id);
                        _onElementRemoved.OnNext(new CollectionElementChangedEvent<T>
                        {
                            Index = entity.Id,
                            OldValue = currentValue,
                            NewValue = default(T)
                        });
                    }                    
                    continue;
                }

                var transformedData = Transform(entity);
                if (FilteredCache.ContainsKey(entity.Id))
                {
                    var currentData = FilteredCache[entity.Id];
                    FilteredCache[entity.Id] = transformedData;
                    _onElementChanged.OnNext(new CollectionElementChangedEvent<T>
                    {
                        Index = entity.Id,
                        OldValue = currentData,
                        NewValue = transformedData
                    });
                    continue;
                }

                FilteredCache.Add(entity.Id, transformedData);
                _onElementAdded.OnNext(new CollectionElementChangedEvent<T>
                {
                    Index = entity.Id,
                    OldValue = default(T),
                    NewValue = transformedData
                });
            }
            
            _onDataChanged.OnNext(FilteredCache.Values);
            _needsUpdate = false;
        }

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
        /// The method to see if this entity should be transformed
        /// </summary>
        /// <param name="entity">The entity to verify</param>
        /// <returns>true if it should transform the entity, false if not</returns>
        public abstract bool ShouldTransform(IEntity entity);
        
        /// <summary>
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="entity">The entity to transform</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IEntity entity);
        
        /// <summary>
        /// Available as a way to post process the data, i.e order them
        /// </summary>
        /// <param name="data">Data to transform</param>
        /// <returns>Processed data</returns>
        public virtual IEnumerable<T> PostProcess(IEnumerable<T> data)
        { return data; }

        public IEnumerable<T> GetData()
        {
            if(_needsUpdate)
            { RefreshData(); }
            
            return PostProcess(FilteredCache.Values);
        }

        public IEnumerator<T> GetEnumerator()
        { return GetData().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged?.Dispose();
            _onElementAdded?.Dispose();
            _onElementChanged?.Dispose();
            _onElementRemoved?.Dispose();
        }
    }
}