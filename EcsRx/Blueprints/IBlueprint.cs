using EcsRx.Entities;

namespace EcsRx.Blueprints
{
    public interface IBlueprint
    {
        void Apply(IEntity entity);
    }
}