using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.HelloWorldExample.Components
{
    public class CanTalkComponent : IComponent
    {
        public string Message { get; set; }
    }
}