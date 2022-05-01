using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupBatchTracker : IObservableGroupTracker
    {
        bool StartTrackingEntity(IEntity entity);
        void StopTrackingEntity(IEntity entity);
        bool IsMatching(int entityId);
    }
}