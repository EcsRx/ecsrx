using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using EcsRx.Collections;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public class Struct5DApplication : BasicLoopApplication
    {
        private int[] entityIds;
        private ComponentPool<StructComponent> basic1;
        private ComponentPool<StructComponent2> basic2;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            entityIds = _collection.Select(x => x.Id).ToArray();

            var cd = (ComponentDatabase) _componentDatabase;
            basic1 = (ComponentPool<StructComponent>)cd.ComponentData[StructComponent1TypeId];
            basic2 = (ComponentPool<StructComponent2>)cd.ComponentData[StructComponent2TypeId];
        }

        protected override string Description { get; } = "Same as previous, using unsafe pointers for quicker access read/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            var basicProxy = new ProxyArray2<StructComponent>(_collection.Count, entityIds, basic1.Components);
            var basicProxy2 = new ProxyArray2<StructComponent2>(_collection.Count, entityIds, basic2.Components);
            
            for (var i = entityIds.Length - 1; i >= 0; i--)
            {
                var basicComponent = basicProxy[i];
                var basicComponent2 = basicProxy2[i];

                RunEntity(entityIds[i], ref basicComponent, ref basicComponent2);
                basicProxy[i] = basicComponent;
                basicProxy2[i] = basicComponent2;
            }
        }

        protected void RunEntity(int entityId, ref StructComponent a, ref StructComponent2 b)
        {
            a.Position += Vector3.One;
            a.Something += 10;
            b.Value += 10;
            b.IsTrue = true;
        }
    }
}