using EcsRx.Collections;

namespace EcsRx.Components.Database
{
    public interface IComponentDatabase
    {
        T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent;
        ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent;
        T[] GetComponents<T>(int componentTypeId) where T : IComponent;
        void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent;
        void Remove(int componentTypeId, int allocationIndex);
        int Allocate(int componentTypeId);

        void PreAllocateComponents(int componentTypeId, int allocationSize);
        IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent;
    }
}