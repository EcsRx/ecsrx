namespace EcsRx.Entities
{
    public interface IIdPool : IPool<int>
    {
        bool IsAvailable(int id);
        void ClaimSpecific(int id);
    }
}