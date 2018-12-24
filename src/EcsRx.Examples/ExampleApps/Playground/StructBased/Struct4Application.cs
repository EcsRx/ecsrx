using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.ClassBased;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Groups.Batches;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct4Application : BasicLoopApplication
    {
        private IComponentBatches<CustomStructBatch> _componentBatch;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var manualBatch = _batchManager.GetBatch<CustomStructBatch>();
            manualBatch.InitializeBatches(_collection);
            _componentBatch = manualBatch;
        }

        protected override string Description { get; } = "Uses auto batching to group components for quicker reads, but larger overhead in sync structs";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = _componentBatch.Batches.Length - 1; i >= 0; i--)
            {
                ref var batch = ref _componentBatch.Batches[i];
                batch.Basic.Position += Vector3.One;
                batch.Basic.Something += 10;
                batch.Basic2.IsTrue = true;
                batch.Basic2.Value += 10;
                
                var entity = _collection.GetEntity(batch.EntityId);
                entity.UpdateComponent(StructComponent1TypeId, batch.Basic);
                entity.UpdateComponent(StructComponent2TypeId, batch.Basic2);
            }
        }
    }
}