namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupIndividualTracker : IObservableGroupTracker
    {
        bool IsMatching();
    }
}