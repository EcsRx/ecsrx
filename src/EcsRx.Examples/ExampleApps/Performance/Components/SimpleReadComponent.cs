using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.Performance.Components
{
    public class SimpleReadComponent : IComponent
    {
        public float StartingValue { get; set; } = 100.0f;
    }
}