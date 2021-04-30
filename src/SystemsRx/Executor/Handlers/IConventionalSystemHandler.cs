using System;
using SystemsRx.Systems;

namespace SystemsRx.Executor.Handlers
{
    public interface IConventionalSystemHandler : IDisposable
    {
        bool CanHandleSystem(ISystem system);
        void SetupSystem(ISystem system);
        void DestroySystem(ISystem system);
    }
}