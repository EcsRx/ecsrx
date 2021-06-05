using SystemsRx.Pools;

namespace EcsRx.Plugins.Views.Pooling
{
    public interface IViewPool : IPool<object>
    {
        void PreAllocate(int allocationCount);
        void DeAllocate(int dellocationCount);
        void EmptyPool();
    }
}