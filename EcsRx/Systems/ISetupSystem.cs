using EcsRx.Entities;

namespace EcsRx.Systems
{
    public interface ISetupSystem : ISystem
    {
        void Setup(IEntity entity);
    }
}