using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Groups.Batches;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Class4Application : BasicLoopApplication
    {
        private IComponentBatches<CustomClassBatch> _componentBatch;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(ClassComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(ClassComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var manualBatch = _batchManager.GetBatch<CustomClassBatch>();
            manualBatch.InitializeBatches(_collection);
            _componentBatch = manualBatch;
        }

        protected override string Description { get; } = "Uses auto batching to allow the components to be clustered better in memory";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<ClassComponent>(ClassComponent1TypeId);
            entity.AddComponent<ClassComponent2>(ClassComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = _componentBatch.Batches.Length - 1; i >= 0; i--)
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