using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupBatchTracker : IObservableGroupTracker
    {
        void StartTrackingEntity(IEntity entity);
        void StopTrackingEntity(IEntity entity);
    }
}