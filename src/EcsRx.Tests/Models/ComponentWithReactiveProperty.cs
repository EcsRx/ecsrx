using System;
using EcsRx.Components;
using SystemsRx.ReactiveData;

namespace EcsRx.Tests.Models
{
    public class ComponentWithReactiveProperty : IComponent, IDisposable
    {
        public ReactiveProperty<int> SomeNumber { get; } = new ReactiveProperty<int>();

        public void Dispose()
        { SomeNumber?.Dispose(); }
    }
}