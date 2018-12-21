using System.Numerics;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Playground.ClassBased
{
    public class Class3Application : BasicLoopApplication
    {
        protected ClassComponent[] Basic1Components;
        protected ClassComponent2[] Basic2Components;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(ClassComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(ClassComponent2TypeId, EntityCount);
            base.SetupEntities();

            Basic1Components = _componentDatabase.GetComponents<ClassComponent>(ClassComponent1TypeId);
            Basic2Components = _componentDatabase.GetComponents<ClassComponent2>(ClassComponent2TypeId);
        }

        protected override string Description { get; } =
            "Pre allocating components and caching component arrays for quicker lookups";

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
                var basicComponent = Basic1Components[entity.ComponentAllocations[ClassComponent1TypeId]];
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
                
                var basicComponent2 = Basic2Components[entity.ComponentAllocations[ClassComponent2TypeId]];
                basicComponent2.Value += 10;
                basicComponent2.IsTrue = true;
            }
        }
    }
}