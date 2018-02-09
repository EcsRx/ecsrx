using System;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface IConventionalSystemHandler<in T> : IDisposable where T : ISystem
    {
        bool CanHandleSystem(ISystem system);
        void SetupSystem(T system);
        void DestroySystem(T system);
    }
}