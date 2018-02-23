using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;

namespace EcsRx.Views.ViewHandlers
{
    public interface IViewHandler
    {
        IPoolManager PoolManager { get; }
        IEventSystem EventSystem { get; }

        void DestroyView(object view);
        void SetupView(IEntity entity);
    }
}