using System;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.Plugins.Batching.Batches;
using EcsRx.Plugins.Batching.Builders;

namespace EcsRx.Plugins.Batching.Accessors
{
    public abstract class BatchAccessor : IBatchAccessor, IDisposable
    {
        public IObservableGroup ObservableGroup { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IBatchBuilder BatchBuilder { get; }

        protected IDisposable Subscriptions;
        
        protected BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup)
        {
            ObservableGroup = observableGroup;
            ComponentDatabase = componentDatabase;
            BatchBuilder = batchBuilder;
            ComponentTypeLookup = componentTypeLookup;

            SetupAccessor();
        }

        protected void SetupAccessor()
        {
            var subscriptions = new CompositeDisposable();
            ObservableGroup.OnEntityAdded.Subscribe(_ => Refresh()).AddTo(subscriptions);
            ObservableGroup.OnEntityRemoved.Subscribe(_ => Refresh()).AddTo(subscriptions);
            ReactToPools().AddTo(subscriptions);
            Subscriptions = subscriptions;

            Refresh();
        }

        protected abstract IDisposable ReactToPools();
        public abstract void Refresh();

        public void Dispose()
        { Subscriptions?.Dispose(); }
    }

    public class BatchAccessor<T1, T2> : BatchAccessor, IBatchAccessor<T1, T2> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public Batch<T1, T2>[] Batch { get; private set; }

        public BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) 
            : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {}

        protected override IDisposable ReactToPools()
        {
            var componentType1 = ComponentTypeLookup.GetComponentType(typeof(T1));
            var componentType2 = ComponentTypeLookup.GetComponentType(typeof(T2));
            
            var pool1 = ComponentDatabase.GetPoolFor<T1>(componentType1);
            var pool2 = ComponentDatabase.GetPoolFor<T2>(componentType2);
            
            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IBatchBuilder<T1, T2>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class BatchAccessor<T1, T2, T3> : BatchAccessor, IBatchAccessor<T1, T2, T3> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public Batch<T1, T2, T3>[] Batch { get; private set; }

        public BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) 
            : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {}

        protected override IDisposable ReactToPools()
        {
            var componentType1 = ComponentTypeLookup.GetComponentType(typeof(T1));
            var componentType2 = ComponentTypeLookup.GetComponentType(typeof(T2));
            var componentType3 = ComponentTypeLookup.GetComponentType(typeof(T3));
            
            var pool1 = ComponentDatabase.GetPoolFor<T1>(componentType1);
            var pool2 = ComponentDatabase.GetPoolFor<T2>(componentType2);
            var pool3 = ComponentDatabase.GetPoolFor<T3>(componentType3);
            
            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool3.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IBatchBuilder<T1, T2, T3>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class BatchAccessor<T1, T2, T3, T4> : BatchAccessor, IBatchAccessor<T1, T2, T3, T4> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        public Batch<T1, T2, T3, T4>[] Batch { get; private set; }

        public BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) 
            : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {}

        protected override IDisposable ReactToPools()
        {
            var componentType1 = ComponentTypeLookup.GetComponentType(typeof(T1));
            var componentType2 = ComponentTypeLookup.GetComponentType(typeof(T2));
            var componentType3 = ComponentTypeLookup.GetComponentType(typeof(T3));
            var componentType4 = ComponentTypeLookup.GetComponentType(typeof(T4));
            
            var pool1 = ComponentDatabase.GetPoolFor<T1>(componentType1);
            var pool2 = ComponentDatabase.GetPoolFor<T2>(componentType2);
            var pool3 = ComponentDatabase.GetPoolFor<T3>(componentType3);
            var pool4 = ComponentDatabase.GetPoolFor<T4>(componentType4);
            
            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool3.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool4.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IBatchBuilder<T1, T2, T3, T4>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class BatchAccessor<T1, T2, T3, T4, T5> : BatchAccessor, IBatchAccessor<T1, T2, T3, T4, T5> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        public Batch<T1, T2, T3, T4, T5>[] Batch { get; private set; }

        public BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) 
            : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {}

        protected override IDisposable ReactToPools()
        {
            var componentType1 = ComponentTypeLookup.GetComponentType(typeof(T1));
            var componentType2 = ComponentTypeLookup.GetComponentType(typeof(T2));
            var componentType3 = ComponentTypeLookup.GetComponentType(typeof(T3));
            var componentType4 = ComponentTypeLookup.GetComponentType(typeof(T4));
            var componentType5 = ComponentTypeLookup.GetComponentType(typeof(T5));
            
            var pool1 = ComponentDatabase.GetPoolFor<T1>(componentType1);
            var pool2 = ComponentDatabase.GetPoolFor<T2>(componentType2);
            var pool3 = ComponentDatabase.GetPoolFor<T3>(componentType3);
            var pool4 = ComponentDatabase.GetPoolFor<T4>(componentType4);
            var pool5 = ComponentDatabase.GetPoolFor<T4>(componentType5);
            
            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool3.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool4.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool5.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IBatchBuilder<T1, T2, T3, T4, T5>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class BatchAccessor<T1, T2, T3, T4, T5, T6> : BatchAccessor, IBatchAccessor<T1, T2, T3, T4, T5, T6> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        public Batch<T1, T2, T3, T4, T5, T6>[] Batch { get; private set; }

        public BatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) 
            : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {}

        protected override IDisposable ReactToPools()
        {
            var componentType1 = ComponentTypeLookup.GetComponentType(typeof(T1));
            var componentType2 = ComponentTypeLookup.GetComponentType(typeof(T2));
            var componentType3 = ComponentTypeLookup.GetComponentType(typeof(T3));
            var componentType4 = ComponentTypeLookup.GetComponentType(typeof(T4));
            var componentType5 = ComponentTypeLookup.GetComponentType(typeof(T5));
            var componentType6 = ComponentTypeLookup.GetComponentType(typeof(T6));
            
            var pool1 = ComponentDatabase.GetPoolFor<T1>(componentType1);
            var pool2 = ComponentDatabase.GetPoolFor<T2>(componentType2);
            var pool3 = ComponentDatabase.GetPoolFor<T3>(componentType3);
            var pool4 = ComponentDatabase.GetPoolFor<T4>(componentType4);
            var pool5 = ComponentDatabase.GetPoolFor<T4>(componentType5);
            var pool6 = ComponentDatabase.GetPoolFor<T4>(componentType6);
            
            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool3.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool4.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool5.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool6.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IBatchBuilder<T1, T2, T3, T4, T5, T6>) BatchBuilder).Build(ObservableGroup); }
    }
}