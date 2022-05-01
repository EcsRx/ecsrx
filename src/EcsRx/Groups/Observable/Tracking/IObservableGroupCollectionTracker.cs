namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupCollectionTracker : IObservableGroupTracker
    {
        bool IsMatching(int entityId);
    }
}