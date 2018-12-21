using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct2Application : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            base.SetupEntities();
        }

        protected override string Description { get; } =
            "Improved by pre-allocating components and using component type ids with structs";

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
                var basicComponent = entity.GetComponent<StructComponent>(StructComponent1TypeId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;

                var basicComponent2 = entity.GetComponent<StructComponent2>(StructComponent2TypeId);
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}
    