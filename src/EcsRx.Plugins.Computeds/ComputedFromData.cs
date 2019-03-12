using System;
using System.Collections.Generic;
using EcsRx.Extensions;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Plugins.Computeds
{
    public abstract class ComputedFromData<TOutput,TInput> : IComputed<TOutput>, IDisposable
    {
        public TOutput CachedData;
        public readonly List<IDisposable> Subscriptions;
        
        private readonly Subject<TOutput> _onDataChanged;
        private bool _needsUpdate;
        
        public TInput DataSource { get; }

        public ComputedFromData(TInput dataSource)
        {
            DataSource = dataSource;
            Subscriptions = new List<IDisposable>();
            
            _onDataChanged = new Subject<TOutput>();

            MonitorChanges();
            RefreshData();
        }
                
        public IDisposable Subscribe(IObserver<TOutput> observer)
        { return _onDataChanged.Subscribe(observer); }

        public TOutput Value => GetData();

        public void MonitorChanges()
        {
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
            var newData = Transform(DataSource);
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
        /// <param name="dataSource">The source of data to work off</param>
        /// <returns>The transformed data</returns>
        public abstract TOutput Transform(TInput dataSource);

        public TOutput GetData()
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