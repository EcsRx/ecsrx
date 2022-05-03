namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public interface ICollectionObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching(int entityId);
    }
}