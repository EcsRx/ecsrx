using System;
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
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ProxyArray2<T> where T : unmanaged
    {
        private T*[] _data;

        public ProxyArray2(int length, int[] indexes, T[] originalData)
        {
            _data = new T*[length];
            var currentIndex = 0;

            fixed (T* elements = originalData)
            {
                for (var i = 0; i < indexes.Length; i++)
                {
                    _data[currentIndex++] = &elements[indexes[i]];
                }
            }
        }
        
        public T this[int index]
        {
            get => *_data[index];
            set => *_data[index] = value;
        }
    }
    
    public class Struct5CApplication : BasicLoopApplication
    {
        private int[] entityIds;
        private ComponentPool basic1, basic2;
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            entityIds = _collection.Select(x => x.Id).ToArray();

            var cd = (ComponentDatabase) _componentDatabase;
            basic1 = (ComponentPool)cd.ComponentData[StructComponent1TypeId];
            basic2 = (ComponentPool)cd.ComponentData[StructComponent2TypeId];
        }

        protected override string Description { get; } = "Same as previous, using unsafe pointers for quicker access read/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            var basicProxy = new ProxyArray2<StructComponent>(_collection.Count, entityIds, (StructComponent[])basic1.Data);
            var basicProxy2 = new ProxyArray2<StructComponent2>(_collection.Count, entityIds, (StructComponent2[])basic2.Data);
            
            for (var i = entityIds.Length - 1; i >= 0; i--)
            {
                var basicComponent = basicProxy[i];
                basicProxy[i] = new StructComponent
                {
                    Position = basicComponent.Position + Vector3.One,
                    Something = basicComponent.Something + 10
                };
                
                var basicComponent2 = basicProxy2[i];
                basicProxy2[i] = new StructComponent2
                {
                    Value = basicComponent2.Value + 10,
                    IsTrue = true
                };
            }
        }
    }
}