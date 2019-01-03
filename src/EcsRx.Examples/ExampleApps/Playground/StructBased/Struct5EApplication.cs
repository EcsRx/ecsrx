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
    public class Struct5EApplication : BasicLoopApplication
    {
        private int[] entityIds;
        private ProxyArray2<StructComponent> basic1;
        private ProxyArray2<StructComponent2> basic2;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            entityIds = _collection.Select(x => x.Id).ToArray();

            var cd = (ComponentDatabase) _componentDatabase;
            var a = (ComponentPool)cd.ComponentData[StructComponent1TypeId];
            var b = (ComponentPool)cd.ComponentData[StructComponent2TypeId];
            
            basic1 = new ProxyArray2<StructComponent>(_collection.Count, entityIds, (StructComponent[])a.Data);
            basic2 = new ProxyArray2<StructComponent2>(_collection.Count, entityIds, (StructComponent2[])b.Data);
        }

        protected override string Description { get; } = "Same as previous, using unsafe pointers for quicker access read/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = entityIds.Length - 1; i >= 0; i--)
            {
                var basicComponent = basic1[i];
                var basicComponent2 = basic2[i];

                RunEntity(entityIds[i], ref basicComponent, ref basicComponent2);
                basic1[i] = basicComponent;
                basic2[i] = basicComponent2;
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