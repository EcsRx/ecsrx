using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;

namespace EcsRx.Computed
{
    public abstract class ComputedGroupCollectionData<T> : IComputedGroupCollectionData<T>
    {
        public IDictionary<int, T> FilteredCache { get; }
        public List<IDisposable> Subscriptions { get; }
        
        public IObservable<IEnumerable<T>> OnDataChanged => _onDataChanged;

        private readonly Subject<IEnumerable<T>> _onDataChanged;
        private bool _needsUpdate;
        
        public IObservableGroup InternalObservableGroup { get; }

        public ComputedGroupCollectionData(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();       
            FilteredCache = new Dictionary<int, T>();
            _onDataChanged = new Subject<IEnumerable<T>>();

            MonitorChanges();
            RefreshData();
        }
        
        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(RequestUpdate).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(RequestUpdate).AddTo(Subscriptions);
            RefreshWhen().Subscribe(x => RequestUpdate()).AddTo(Subscriptions);
        }
        
        public void RequestUpdate(object _ = null)
        { _needsUpdate = true; }
        
        public void RefreshData()
        {
            foreach (var entity in InternalObservableGroup)
            {
                var isApplicable = ShouldTransform(entity);

                if (!isApplicable)
                {
                    FilteredCache.Remove(entity.Id);
                    continue;
                }

                var transformedData = Transform(entity);
                if(FilteredCache.ContainsKey(entity.Id))
                { FilteredCache[entity.Id] = transformedData; }
                else
                { FilteredCache.Add(entity.Id, transformedData); }
            }
            
            _onDataChanged.OnNext(FilteredCache.Values);
            _needsUpdate = false;
        }

        public abstract IObservable<bool> RefreshWhen();     
        public abstract bool ShouldTransform(IEntity entity);
        public abstract T Transform(IEntity entity);
        
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
    }
}