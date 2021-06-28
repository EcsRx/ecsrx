using System;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using EcsRx.Systems;

namespace EcsRx.Plugins.Batching.Systems
{
    public abstract class ManualBatchedSystem : IManualSystem 
    {
        public abstract IGroup Group { get; }
        
        public IObservableGroupManager ObservableGroupManager { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IThreadHandler ThreadHandler { get; }
        
        protected IObservableGroup ObservableGroup { get; private set; }
        protected bool ShouldParallelize { get; private set; }
        protected IDisposable Subscriptions;

        protected ManualBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            ThreadHandler = threadHandler;
            ObservableGroupManager = observableGroupManager;
        }

        protected abstract void RebuildBatch();
        
        /// <summary>
        /// This describes when the system should be processed
        /// </summary>
        /// <returns>A trigger indicating that the process should run</returns>
        protected abstract IObservable<bool> ReactWhen();

        /// <summary>
        /// Do anything before the batch gets processed
        /// </summary>
        protected virtual void BeforeProcessing(){}
        
        /// <summary>
        /// Do anything after the batch has been processed
        /// </summary>
        protected virtual void AfterProcessing(){}
        
        /// <summary>
        /// The wrapper for processing the underlying batch
        /// </summary>
        protected abstract void ProcessBatch();

        public virtual void StartSystem()
        {
            ObservableGroup = ObservableGroupManager.GetObservableGroup(Group);
            ShouldParallelize = this.ShouldMutliThread();
            
            var subscriptions = new CompositeDisposable();
            ProcessGroupSubscription(ObservableGroup.OnEntityAdded)
                .Subscribe(_ => RebuildBatch())
                .AddTo(subscriptions);
           
            ProcessGroupSubscription(ObservableGroup.OnEntityRemoved)
                .Subscribe(_ => RebuildBatch())
                .AddTo(subscriptions);
            
            RebuildBatch();
            ReactWhen().Subscribe(_ => RunBatch()).AddTo(subscriptions);
            
            Subscriptions = subscriptions;
        }

        /// <summary>
        /// This processes the group level subscription, allowing you to change how the change of a group should be run 
        /// </summary>
        /// <param name="groupChange"></param>
        /// <returns>The observable stream that should be subscribed to</returns>
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

        public virtual void StopSystem()
        { Subscriptions.Dispose(); }
    }
}