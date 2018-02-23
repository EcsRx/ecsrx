namespace EcsRx.Views.Pooling
{
    public interface IViewPool
    {
        int IncrementSize { get; }

        void PreAllocate(int allocationCount);
        void DeAllocate(int dellocationCount);
        void EmptyPool();

        object AllocateInstance();
        void ReleaseInstance(object instance);
    }
}