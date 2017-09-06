using EcsRx.Entities;

namespace EcsRx.Systems
{
    public interface ITeardownSystem : ISystem
    {
        void Teardown(IEntity entity);
    }
}