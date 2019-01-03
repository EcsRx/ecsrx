using System.Collections.Specialized;
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
    public unsafe struct A<T1,T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public readonly int Id;
        public readonly T1* a;
        public readonly T2* b;

        public A(int id, T1* a, T2* b)
        {
            this.Id = id;
            this.a = a;
            this.b = b;
        }
    }
    
    public unsafe class Struct5FApplication : BasicLoopApplication
    {
        private int[] entityIds;
        private A<StructComponent, StructComponent2>[] basic3;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            entityIds = _collection.Select(x => x.Id).ToArray();

            RebuildBatch();
        }

        protected void RebuildBatch()
        {
            var cd = (ComponentDatabase) _componentDatabase;
            var a = (ComponentPool)cd.ComponentData[StructComponent1TypeId];
            var b = (ComponentPool)cd.ComponentData[StructComponent2TypeId];

            var a1 = (StructComponent[]) a.Data;
            var b1 = (StructComponent2[]) b.Data;
            
            basic3 = new A<StructComponent, StructComponent2>[entityIds.Length];

            fixed (StructComponent* a2 = a1)
            fixed (StructComponent2* b2 = b1)
            {
                for (var i = 0; i < _collection.Count; i++)
                {
                    var entity = _collection[i];
                    var a3 = entity.ComponentAllocations[StructComponent1TypeId];
                    var b3 = entity.ComponentAllocations[StructComponent2TypeId];
                    basic3[i] = new A<StructComponent, StructComponent2>(entity.Id, &a2[a3], &b2[b3]);
                }
            }
        }

        protected override string Description { get; } = "Same as previous, using unsafe pointers for quicker access read/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            for (var i = 0; i < basic3.Length; i++)
            {
                var chunk = basic3[i];
                RunEntity(chunk.Id, ref *chunk.a, ref *chunk.b);
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