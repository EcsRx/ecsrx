using System;
using SystemsRx.Extensions;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using EcsRx.Plugins.Batching.Batches;
using EcsRx.Plugins.Batching.Builders;

namespace EcsRx.Plugins.Batching.Accessors
{
    public class ReferenceBatchAccessor<T1, T2> : BatchAccessor, IReferenceBatchAccessor<T1, T2>
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public ReferenceBatch<T1, T2>[] Batch { get; private set;  }

        public ReferenceBatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {
        }

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
        { Batch = ((IReferenceBatchBuilder<T1, T2>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class ReferenceBatchAccessor<T1, T2, T3> : BatchAccessor, IReferenceBatchAccessor<T1, T2, T3>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        public ReferenceBatch<T1, T2, T3>[] Batch { get; private set;  }

        public ReferenceBatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {
        }

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
        { Batch = ((IReferenceBatchBuilder<T1, T2, T3>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class ReferenceBatchAccessor<T1, T2, T3, T4> : BatchAccessor, IReferenceBatchAccessor<T1, T2, T3, T4>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        public ReferenceBatch<T1, T2, T3, T4>[] Batch { get; private set;  }

        public ReferenceBatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {
        }

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
        { Batch = ((IReferenceBatchBuilder<T1, T2, T3, T4>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class ReferenceBatchAccessor<T1, T2, T3, T4, T5> : BatchAccessor, IReferenceBatchAccessor<T1, T2, T3, T4, T5>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        public ReferenceBatch<T1, T2, T3, T4, T5>[] Batch { get; private set;  }

        public ReferenceBatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {
        }

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
            var pool5 = ComponentDatabase.GetPoolFor<T5>(componentType5);

            var subscriptions = new CompositeDisposable();
            pool1.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool2.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool3.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool4.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);
            pool5.OnPoolExtending.Subscribe(_ => Refresh()).AddTo(subscriptions);

            return subscriptions;
        }

        public override void Refresh()
        { Batch = ((IReferenceBatchBuilder<T1, T2, T3, T4, T5>) BatchBuilder).Build(ObservableGroup); }
    }
    
    public class ReferenceBatchAccessor<T1, T2, T3, T4, T5, T6> : BatchAccessor, IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6>
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        public ReferenceBatch<T1, T2, T3, T4, T5, T6>[] Batch { get; private set;  }

        public ReferenceBatchAccessor(IObservableGroup observableGroup, IComponentDatabase componentDatabase, IBatchBuilder batchBuilder, IComponentTypeLookup componentTypeLookup) : base(observableGroup, componentDatabase, batchBuilder, componentTypeLookup)
        {
        }

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
            var pool5 = ComponentDatabase.GetPoolFor<T5>(componentType5);
            var pool6 = ComponentDatabase.GetPoolFor<T6>(componentType6);

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
        { Batch = ((IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6>) BatchBuilder).Build(ObservableGroup); }
    }
}