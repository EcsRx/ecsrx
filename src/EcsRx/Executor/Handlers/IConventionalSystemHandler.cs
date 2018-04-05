using System;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface IConventionalSystemHandler : IDisposable
    {
        bool CanHandleSystem(ISystem system);
        void SetupSystem(ISystem system);
        void DestroySystem(ISystem system);
    }
}