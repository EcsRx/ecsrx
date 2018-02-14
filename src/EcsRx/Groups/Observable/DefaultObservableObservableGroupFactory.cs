using EcsRx.Events;

namespace EcsRx.Groups.Accessors
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
            var groupAccessor = new ObservableGroup(_eventSystem, arg.ObservableGroupToken, arg.InitialEntities);
            return groupAccessor;            
        }
    }
}