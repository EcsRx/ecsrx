namespace EcsRx.Infrastructure.Dependencies
{
    public interface IDependencyModule
    {
        void Setup(IDependencyContainer container);
    }
}