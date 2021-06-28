using System;
using SystemsRx.ReactiveData;
using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.HealthExample.Components
{
    public class HealthComponent : IComponent, IDisposable
    {
        public ReactiveProperty<float> Health { get; set; }
        public float MaxHealth { get; set; }
        
        public void Dispose()
        {
            Health.Dispose();
        }
    }
}