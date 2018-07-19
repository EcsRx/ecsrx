using System;
using System.Collections.Generic;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;

namespace EcsRx.Computed
{
    public abstract class ComputedGroupData<T> : IComputedGroupData<T>, IDisposable
    {
        public T CachedData;
        public readonly List<IDisposable> Subscriptions;
        
        public IObservable<T> OnDataChanged => _onDataChanged;

        private readonly Subject<T> _onDataChanged;
        private bool _needsUpdate;
        
        public IObservableGroup InternalObservableGroup { get; }

        public ComputedGroupData(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();
            
            _onDataChanged = new Subject<T>();

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
            var newData = Filter(InternalObservableGroup);
            if (newData.Equals(CachedData)) { return; }
            
            CachedData = newData;
            _onDataChanged.OnNext(CachedData);
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
        /// The method to check if the entity is applicable to this computed group
        /// </summary>
        /// <param name="observableGroup">The group to process</param>
        /// <returns>The filtered data</returns>
        public abstract T Filter(IObservableGroup observableGroup);

        public T GetData()
        {
            if(_needsUpdate)
            { RefreshData(); }

            return CachedData;
        }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}