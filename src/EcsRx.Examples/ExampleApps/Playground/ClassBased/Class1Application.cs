using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Playground.ClassBased
{
    public class Class1Application : BasicLoopApplication
    {
        protected override string Description { get; } =
            "Simplest possible approach, no pre allocation, using generics";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<ClassComponent>();
            entity.AddComponent<ClassComponent2>();
        }

        protected override void RunProcess()
        {/*
            foreach (var entity in _collection)
            {
                var basicComponent = entity.GetComponent<ClassComponent>();
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;

                var basicComponent2 = entity.GetComponent<ClassComponent2>();
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }*/
        }
    }
}