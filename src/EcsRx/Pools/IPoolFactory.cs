using EcsRx.Factories;

namespace EcsRx.Pools
{
    public interface IPoolFactory : IFactory<string, IPool>
    {
        
    }
}