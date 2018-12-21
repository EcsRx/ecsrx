using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

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