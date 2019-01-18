using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Playground.ClassBased
{
    public class Class2Application : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(ClassComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(ClassComponent2TypeId, EntityCount);
            base.SetupEntities();
        }

        protected override string Description { get; } =
            "Improved by pre-allocating components and using component type ids";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<ClassComponent>();
            entity.AddComponent<ClassComponent2>();
        }

        protected override void RunProcess()
        {
            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                var basicComponent = entity.GetComponent<ClassComponent>(ClassComponent1TypeId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
                
                var basicComponent2 = entity.GetComponent<ClassComponent2>(ClassComponent2TypeId);
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}