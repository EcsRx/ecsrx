namespace EcsRx.Entities
{
    public interface IIdPool : IPool<int>
    {
        bool IsAvailable(int id);
        void AllocateSpecificId(int id);
    }
}