using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public interface IBatchObservableGroupTracker : ICollectionObservableGroupTracker
    {
        bool StartTrackingEntity(IEntity entity);
        void StopTrackingEntity(IEntity entity);
    }
}