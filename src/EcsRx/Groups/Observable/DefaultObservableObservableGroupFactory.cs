using EcsRx.Groups.Observable.Tracking;

namespace EcsRx.Groups.Observable
{
    public class DefaultObservableObservableGroupFactory : IObservableGroupFactory
    {
        private IGroupTrackerFactory GroupTrackerFactory { get; }

        public DefaultObservableObservableGroupFactory(IGroupTrackerFactory groupTrackerFactory)
        {
            GroupTrackerFactory = groupTrackerFactory;
        }

        public IObservableGroup Create(ObservableGroupConfiguration arg)
        {
            return new ObservableGroup(arg.ObservableGroupToken, arg.InitialEntities, arg.NotifyingCollections, GroupTrackerFactory);
        }
    }
}