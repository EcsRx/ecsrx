using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Systems
{
    public interface IManualSystem : ISystem
    {
        void StartSystem(IObservableGroup observableGroup);
        void StopSystem(IObservableGroup observableGroup);
    }
}