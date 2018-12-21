using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct4Application : BasicLoopApplication
    {
        private CustomBatch[] Batches;        
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            
            var BasicArray = _componentDatabase.GetComponents<StructComponent>(StructComponent1TypeId);
            var BasicArray2 = _componentDatabase.GetComponents<StructComponent2>(StructComponent2TypeId);

            Batches = new CustomBatch[_collection.Count];
            for (var i = 0; i < _collection.Count; i++)
            {
                var entity = _collection[i];
                var batch = new CustomBatch
                {
                    EntityId = entity.Id,
                    Basic = BasicArray[entity.ComponentAllocations[StructComponent1TypeId]],
                    Basic2 = BasicArray2[entity.ComponentAllocations[StructComponent2TypeId]]
                };
                Batches[i] = batch;
            }
        }

        protected override string Description { get; } = "Same as previous example but now adding manual batching";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = Batches.Length - 1; i >= 0; i--)
            {
                var batch = Batches[i];
                batch.Basic.Position += Vector3.One;
                batch.Basic.Something += 10;
                batch.Basic2.IsTrue = true;
                batch.Basic2.Value += 10;
            }
        }
    }
}