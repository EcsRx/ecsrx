using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Plugins.Batching.Batches;


namespace EcsRx.Examples.ExampleApps.Playground.ClassBased
{
    public class Class4Application : BasicLoopApplication
    {
        private ReferenceBatch<ClassComponent, ClassComponent2>[] _componentBatch;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(ClassComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(ClassComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var batchBuilder = _referenceBatchBuilderFactory.Create<ClassComponent, ClassComponent2>();
            _componentBatch = batchBuilder.Build(_collection);
        }

        protected override string Description { get; } = "Uses auto batching to allow the components to be clustered better in memory";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<ClassComponent>(ClassComponent1TypeId);
            entity.AddComponent<ClassComponent2>(ClassComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = _componentBatch.Length - 1; i >= 0; i--)
            {
                var batch = _componentBatch[i];
                var basic = batch.Component1;
                var basic2 = batch.Component2;
                basic.Position += Vector3.One;
                basic.Something += 10;
                basic2.IsTrue = true;
                basic2.Value += 10;
            }
        }
    }
}