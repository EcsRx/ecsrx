using System;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.Systems;
using EcsRx.Threading;

namespace EcsRx.Plugins.Batching.Systems
{
    public abstract class ManualBatchedSystem : IManualSystem 
    {
        public abstract IGroup Group { get; }
        
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IThreadHandler ThreadHandler { get; }
        
        protected IObservableGroup ObservableGroup { get; set; }
        protected bool ShouldParallelize { get; private set; }
        protected IDisposable Subscriptions;

        protected ManualBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IThreadHandler threadHandler)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            ThreadHandler = threadHandler;
        }
        
        protected abstract void RebuildBatch();
        protected abstract IObservable<bool> ReactWhen();

        protected virtual void BeforeProcessing(){}
        protected virtual void AfterProcessing(){}
        protected abstract void ProcessBatch();

        public virtual void StartSystem(IObservableGroup observableGroup)
        {
            ObservableGroup = observableGroup;
            ShouldParallelize = this.ShouldMutliThread();
            
            var subscriptions = new CompositeDisposable();
            ProcessGroupSubscription(ObservableGroup.OnEntityAdded).Subscribe(_ => RebuildBatch()).AddTo(subscriptions);
            ProcessGroupSubscription(ObservableGroup.OnEntityRemoved).Subscribe(_ => RebuildBatch()).AddTo(subscriptions);
            
            RebuildBatch();
            ReactWhen().Subscribe(_ => RunBatch()).AddTo(subscriptions);
            
            Subscriptions = subscriptions;
        }

        /// <summary>
        /// This processes the group level subscription, allowing you to change how the change of a group should be run 
        /// </summary>
        /// <param name="groupChange"></param>
        /// <returns></returns>
        /// <remarks>Out the box it will just pass through the observable but in a lot of cases you may want to
        /// throttle the group changes so multiple ones within a single frame would be run once.</remarks>
        protected virtual IObservable<IEntity> ProcessGroupSubscription(IObservable<IEntity> groupChange)
        { return groupChange; }

        private void RunBatch()
        {
            BeforeProcessing();
            ProcessBatch();
            AfterProcessing();
        }

        public virtual void StopSystem(IObservableGroup observableGroup)
        { Subscriptions.Dispose(); }
    }
}