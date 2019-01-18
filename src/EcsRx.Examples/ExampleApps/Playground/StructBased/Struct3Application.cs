using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct3Application : BasicLoopApplication
    {
        protected StructComponent[] Components1;
        protected StructComponent2[] Components2;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            base.SetupEntities();

            Components1 = _componentDatabase.GetComponents<StructComponent>(StructComponent1TypeId);
            Components2 = _componentDatabase.GetComponents<StructComponent2>(StructComponent2TypeId);
        }

        protected override string Description { get; } =
            "Pre allocating components and caching component arrays for quicker lookups with structs";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }
        
        protected override void RunProcess()
        {
            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                
                ref var basicComponent = ref Components1[entity.ComponentAllocations[StructComponent1TypeId]];
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
                
                ref var basicComponent2 = ref Components2[entity.ComponentAllocations[StructComponent2TypeId]];
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}