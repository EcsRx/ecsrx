using EcsRx.Events;

namespace EcsRx.Groups.Observable
{
    public class DefaultObservableObservableGroupFactory : IObservableGroupFactory
    {
        private readonly IEventSystem _eventSystem;

        public DefaultObservableObservableGroupFactory(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public IObservableGroup Create(ObservableGroupConfiguration arg)
        {
            return new ObservableGroup(_eventSystem, arg.ObservableGroupToken, arg.InitialEntities);      
        }
    }
}