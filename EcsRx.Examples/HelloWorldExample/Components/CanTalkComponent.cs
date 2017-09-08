using EcsRx.Components;

namespace EcsRx.Examples.HelloWorldExample.Components
{
    public class CanTalkComponent : IComponent
    {
        public string Message { get; set; }
    }
}