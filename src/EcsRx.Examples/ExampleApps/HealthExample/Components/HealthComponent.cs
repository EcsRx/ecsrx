using System;
using EcsRx.Components;
using R3;

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