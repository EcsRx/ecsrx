using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Playground.Batches;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.StructBased
{
    public unsafe struct ProxyArray<T> where T : unmanaged
    {
        private T*[] _data;

        public ProxyArray(int length, int[] indexes, ref T[] originalData)
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
    
    public class Struct5BApplication : BasicLoopApplication
    {
        private int[] entityIds;        
        
        protected override void SetupEntities()
        {
            _componentDatabase.PreAllocateComponents(StructComponent1TypeId, EntityCount);
            _componentDatabase.PreAllocateComponents(StructComponent2TypeId, EntityCount);
            
            base.SetupEntities();
            entityIds = _collection.Select(x => x.Id).ToArray();
        }

        protected override string Description { get; } = "Same as previous, using unsafe pointers for quicker access read/writes";

        protected override void SetupEntity(IEntity entity)
        {
            entity.AddComponent<StructComponent>(StructComponent1TypeId);
            entity.AddComponent<StructComponent2>(StructComponent2TypeId);
        }

        protected override void RunProcess()
        {
            var BasicArray = _componentDatabase.GetComponents<StructComponent>(StructComponent1TypeId);
            var BasicArray2 = _componentDatabase.GetComponents<StructComponent2>(StructComponent2TypeId);
            var basicProxy = new ProxyArray<StructComponent>(_collection.Count, entityIds, ref BasicArray);
            var basicProxy2 = new ProxyArray<StructComponent2>(_collection.Count, entityIds, ref BasicArray2);
            
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