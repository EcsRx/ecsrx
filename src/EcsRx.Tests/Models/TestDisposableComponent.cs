using System;
using EcsRx.Components;

namespace EcsRx.Tests.Models
{
    public class TestDisposableComponent : IComponent, IDisposable
    {
        public bool isDisposed = false;

        public void Dispose()
        { isDisposed = true; }
    }
}