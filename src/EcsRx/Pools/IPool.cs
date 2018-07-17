namespace EcsRx.Entities
{
    public interface IPool<T>
    {
        int IncrementSize { get; }
        
        T AllocateInstance();
        void ReleaseInstance(T obj);
    }
}