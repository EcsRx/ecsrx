using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct3CApplication : BasicLoopApplication
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
                
                var component1Allocation = entity.ComponentAllocations[StructComponent1TypeId];
                var basicComponent = Components1[component1Allocation];
                Components1[component1Allocation] = new StructComponent
                {
                    Position = basicComponent.Position + Vector3.One,
                    Something = basicComponent.Something + 10
                };
                
                var component2Allocation = entity.ComponentAllocations[StructComponent2TypeId];
                var basicComponent2 = Components2[component2Allocation];
                Components2[component2Allocation] = new StructComponent2
                {
                    Value = basicComponent2.Value + 10,
                    IsTrue = 1
                };
            }
        }
    }
}