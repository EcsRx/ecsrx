using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    /// <summary>
    /// 
    /// </summary>
    public class Struct1Application : BasicLoopApplication
    {
        protected override string Description { get; } = "Simplest possible approach but with structs";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            foreach (var entity in _collection)
            {
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