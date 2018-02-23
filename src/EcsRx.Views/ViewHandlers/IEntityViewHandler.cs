using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;

namespace EcsRx.Views.ViewHandlers
{
    public interface IEntityViewHandler
    {
        void SetupView(IEntity entity);
    }
}