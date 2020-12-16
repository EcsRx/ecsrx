using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;

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
                ref var basicComponent = ref entity.GetComponent<StructComponent>(StructComponent1TypeId);
                basicComponent.X += Vector3.One.X;
                basicComponent.Y += Vector3.One.Y;
                basicComponent.Z += Vector3.One.Z;
                basicComponent.Something += 10;

                ref var basicComponent2 = ref entity.GetComponent<StructComponent2>(StructComponent2TypeId);
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}
    