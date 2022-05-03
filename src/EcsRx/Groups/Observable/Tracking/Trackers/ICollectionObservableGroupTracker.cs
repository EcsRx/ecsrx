namespace EcsRx.Groups.Observable.Tracking
{
    public interface ICollectionObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching(int entityId);
    }
}