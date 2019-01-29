using System;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Components.Lookups;

namespace EcsRx.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public int DefaultExpansionAmount { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public IComponentPool[] ComponentData { get; private set; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int defaultExpansionSize = 100)
        {
            ComponentTypeLookup = componentTypeLookup;
            DefaultExpansionAmount = defaultExpansionSize;
            Initialize();
        }

        public IComponentPool CreatePoolFor(Type type, int initialSize)
        {
            var componentPoolType = typeof(ComponentPool<>);
            Type[] typeArgs = { type };
            var genericComponentPoolType = componentPoolType.MakeGenericType(typeArgs);
            return (IComponentPool)Activator.CreateInstance(genericComponentPoolType, initialSize);
        }
        
        public void Initialize()
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            ComponentData = new IComponentPool[componentCount];

            for (var i = 0; i < componentCount; i++)
            { ComponentData[i] = CreatePoolFor(componentTypes[i].Key, DefaultExpansionAmount); }            
        }

        public IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent
        { return (IComponentPool<T>) ComponentData[componentTypeId]; }
        
        public T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent
        { return GetPoolFor<T>(componentTypeId).Components[allocationIndex]; }
        
        public ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent
        { return ref GetPoolFor<T>(componentTypeId).Components[allocationIndex]; }

        public T[] GetComponents<T>(int componentTypeId) where T : IComponent
        { return GetPoolFor<T>(componentTypeId).Components; }

        public void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent
        { GetPoolFor<T>(componentTypeId).Components[allocationIndex] = component; }
        
        public void Remove(int componentTypeId, int allocationIndex)
        { ComponentData[componentTypeId].Release(allocationIndex); }

        public int Allocate(int componentTypeId)
        {
            var pool = ComponentData[componentTypeId];
            return pool.Allocate();
        }

        public void PreAllocateComponents(int componentTypeId, int allocationSize)
        {
            var pool = ComponentData[componentTypeId];
            pool.Expand(allocationSize);
        }
    }
}