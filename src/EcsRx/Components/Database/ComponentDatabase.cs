using System.Linq;
using EcsRx.Collections;
using EcsRx.Components.Lookups;

namespace EcsRx.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public int DefaultExpansionAmount { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public IExpandingArrayPool[] ComponentData { get; private set; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int defaultExpansionSize = 100)
        {
            ComponentTypeLookup = componentTypeLookup;
            DefaultExpansionAmount = defaultExpansionSize;
            Initialize();
        }
        
        public void Initialize()
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            ComponentData = new IExpandingArrayPool[componentCount];

            for (var i = 0; i < componentCount; i++)
            { ComponentData[i] = new ExpandingArrayPool(componentTypes[i].Key, DefaultExpansionAmount, DefaultExpansionAmount); }            
        }
        
        public T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent
        { return ComponentData[componentTypeId].Get<T>(allocationIndex); }
        
        public ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent
        { return ref ComponentData[componentTypeId].GetRef<T>(allocationIndex); }

        public T[] GetComponents<T>(int componentTypeId) where T : IComponent
        { return ComponentData[componentTypeId].AsArray<T>(); }

        public void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent
        { ComponentData[componentTypeId].Set(allocationIndex, component); }
        
        public void Remove(int componentTypeId, int allocationIndex)
        { ComponentData[componentTypeId].Release(allocationIndex); }

        public int Allocate(int componentTypeId)
        {
            var pool = ComponentData[componentTypeId];
            if(pool.IndexesRemaining == 0) { pool.Expand(DefaultExpansionAmount); }
            return pool.Allocate();
        }

        public void PreAllocateComponents(int componentTypeId, int allocationSize)
        {
            var pool = ComponentData[componentTypeId];
            pool.Expand(allocationSize);
        }
    }
}