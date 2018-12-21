using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.ClassBased;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Groups.Batches;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct5Application : BasicLoopApplication
    {
        private IComponentBatches<CustomBatch> _componentBatch;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var manualBatch = _batchManager.GetBatch<CustomBatch>();
            manualBatch.RefreshBatches(_collection);
            _componentBatch = manualBatch;
        }

        protected override string Description { get; } = "Uses auto batching to achieve same results as previous with less code";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = _componentBatch.Batches.Count - 1; i >= 0; i--)
            {
                var batch = _componentBatch.Batches[i];
                batch.Basic.Position += Vector3.One;
                batch.Basic.Something += 10;
                batch.Basic2.IsTrue = true;
                batch.Basic2.Value += 10;
            }
        }
    }
}