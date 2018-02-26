using EcsRx.Entities;

namespace EcsRx.Views.ViewHandlers
{
    public interface IEntityViewHandler
    {
        void SetupView(IEntity entity);
    }
}