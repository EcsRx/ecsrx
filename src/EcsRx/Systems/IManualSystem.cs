using EcsRx.Groups.Accessors;
using EcsRx.Groups;

namespace EcsRx.Systems
{
    public interface IManualSystem : ISystem
    {
        void StartSystem(IObservableGroup observableGroup);
        void StopSystem(IObservableGroup observableGroup);
    }
}