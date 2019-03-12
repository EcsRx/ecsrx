using EcsRx.Entities;

namespace EcsRx.Groups
{
    public interface IHasPredicate
    {
        bool CanProcessEntity(IEntity entity);
    }
}