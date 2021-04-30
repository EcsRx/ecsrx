using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Groups;
using EcsRx.Plugins.Batching.Batches;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Plugins.Batching.Systems
{
    public abstract class ReferenceBatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));
        
        private readonly IReferenceBatchBuilder<T1, T2> _batchBuilder;
        protected ReferenceBatch<T1, T2>[] _batches;
        
        protected abstract void Process(int entityId, T1 component1, T2 component2);

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    var batch = _batches[i];
                    Process(batch.EntityId, batch.Component1, batch.Component2);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                var batch = _batches[i];
                Process(batch.EntityId, batch.Component1, batch.Component2);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly IReferenceBatchBuilder<T1, T2, T3> _batchBuilder;
        protected ReferenceBatch<T1, T2, T3>[] _batches;
        
        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3);

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    var batch = _batches[i];
                    Process(batch.EntityId, batch.Component1, batch.Component2, batch.Component3);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                var batch = _batches[i];
                Process(batch.EntityId, batch.Component1, batch.Component2, batch.Component3);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        
        private readonly IReferenceBatchBuilder<T1, T2, T3, T4> _batchBuilder;
        protected ReferenceBatch<T1, T2, T3, T4>[] _batches;
        
        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4);

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    var batch = _batches[i];
                    Process(batch.EntityId, batch.Component1, batch.Component2, 
                        batch.Component3, batch.Component4);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                var batch = _batches[i];
                Process(batch.EntityId, batch.Component1, batch.Component2, 
                    batch.Component3, batch.Component4);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4, T5> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        private readonly IReferenceBatchBuilder<T1, T2, T3, T4, T5> _batchBuilder;
        protected ReferenceBatch<T1, T2, T3, T4, T5>[] _batches;
        
        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    var batch = _batches[i];
                    Process(batch.EntityId, batch.Component1, batch.Component2, 
                        batch.Component3, batch.Component4, batch.Component5);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                var batch = _batches[i];
                Process(batch.EntityId, batch.Component1, batch.Component2, 
                    batch.Component3, batch.Component4, batch.Component5);
            }
        }
    }
    
    public abstract class ReferenceBatchedSystem<T1, T2, T3, T4, T5, T6> : ManualBatchedSystem
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        
        private readonly IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> _batchBuilder;
        protected ReferenceBatch<T1, T2, T3, T4, T5, T6>[] _batches;
        
        protected abstract void Process(int entityId, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5, T6 component6);

        protected ReferenceBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    var batch = _batches[i];
                    Process(batch.EntityId, batch.Component1, batch.Component2, 
                        batch.Component3, batch.Component4, batch.Component5,
                        batch.Component6);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                var batch = _batches[i];
                Process(batch.EntityId, batch.Component1, batch.Component2, 
                    batch.Component3, batch.Component4, batch.Component5,
                    batch.Component6);
            }
        }
    }
}