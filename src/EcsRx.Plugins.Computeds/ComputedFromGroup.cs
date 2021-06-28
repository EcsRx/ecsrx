using System;
using System.Collections.Generic;
using SystemsRx.Computeds;
using SystemsRx.Extensions;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Plugins.Computeds
{
    public abstract class ComputedFromGroup<T> : IComputed<T>, IDisposable
    {
        public T CachedData;
        public readonly List<IDisposable> Subscriptions;
        
        private readonly Subject<T> _onDataChanged;
        private bool _needsUpdate;
        
        public IObservableGroup InternalObservableGroup { get; }

        public ComputedFromGroup(IObservableGroup internalObservableGroup)
        {
            InternalObservableGroup = internalObservableGroup;
            Subscriptions = new List<IDisposable>();
            
            _onDataChanged = new Subject<T>();

            MonitorChanges();
            RefreshData();
        }
                
        public IDisposable Subscribe(IObserver<T> observer)
        { return _onDataChanged.Subscribe(observer); }

        public T Value => GetData();

        public void MonitorChanges()
        {
            InternalObservableGroup.OnEntityAdded.Subscribe(RequestUpdate).AddTo(Subscriptions);
            InternalObservableGroup.OnEntityRemoving.Subscribe(RequestUpdate).AddTo(Subscriptions);
            RefreshWhen().Subscribe(x => RequestUpdate()).AddTo(Subscriptions);
        }

        public void RequestUpdate(object _ = null)
        {
            _needsUpdate = true;
            
            if(_onDataChanged.HasObservers)
            { RefreshData(); }
        }

        public void RefreshData()
        {
            var newData = Transform(InternalObservableGroup);
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
        /// The method to generate given data from the data source
        /// </summary>
        /// <param name="observableGroup">The group to process</param>
        /// <returns>The transformed data</returns>
        public abstract T Transform(IObservableGroup observableGroup);

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