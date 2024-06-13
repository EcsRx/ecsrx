using System;
using System.Collections.Generic;
using EcsRx.Groups.Observable;
using R3;
using SystemsRx.Computeds;
using SystemsRx.Extensions;

namespace EcsRx.Computeds
{
    public abstract class ComputedFromGroup<T> : IComputed<T>, IDisposable
    {
        public T CachedData;
        public readonly List<IDisposable> Subscriptions;
        
        private readonly Subject<T> _onDataChanged;
        
        public IObservableGroup InternalObservableGroup { get; }

        protected ComputedFromGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();
            
            _onDataChanged = new Subject<T>();

            MonitorChanges();
            RefreshData();
        }
                
        public IDisposable Subscribe(Observer<T> observer)
        { return _onDataChanged.Subscribe(observer); }

        public T Value => GetData();

        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(RequestUpdate).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(RequestUpdate).AddTo(Subscriptions);
            RefreshWhen().Subscribe(x => RequestUpdate()).AddTo(Subscriptions);
        }

        public void RequestUpdate(object _ = null)
        { RefreshData(); }

        public void RefreshData()
        {
            var newData = Transform(InternalObservableGroup);
            if (newData.Equals(CachedData)) { return; }
            
            CachedData = newData;
            _onDataChanged.OnNext(CachedData);
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
        public abstract Observable<bool> RefreshWhen();
        
        /// <summary>
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="observableGroup">The group to process</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IObservableGroup observableGroup);

        public T GetData()
        { return CachedData; }

        public virtual void Dispose()
        {
            Subscriptions.DisposeAll();
            _onDataChanged.Dispose();
        }
    }
}