namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public interface IIndividualObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching();
    }
}