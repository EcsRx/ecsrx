using EcsRx.Groups.Accessors;
using EcsRx.Groups;

namespace EcsRx.Systems
{
    public interface IManualSystem : ISystem
    {
        void StartSystem(IGroupAccessor group);
        void StopSystem(IGroupAccessor group);
    }
}