namespace EcsRx.Entities
{
    public interface IPool<T>
    {
        int IncrementSize { get; }
        
        T Claim();
        void Free(T obj);
    }
}