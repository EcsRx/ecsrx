namespace EcsRx.Components
{
    public interface IComponentRepository
    {
        T Get<T>(int id) where T : class, IComponent;
        T Add<T>(T component) where T : class, IComponent;
        void Remove(int id);
    }
}