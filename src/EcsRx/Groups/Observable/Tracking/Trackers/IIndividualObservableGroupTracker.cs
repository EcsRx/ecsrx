namespace EcsRx.Groups.Observable.Tracking
{
    public interface IIndividualObservableGroupTracker : IObservableGroupTracker
    {
        bool IsMatching();
    }
}