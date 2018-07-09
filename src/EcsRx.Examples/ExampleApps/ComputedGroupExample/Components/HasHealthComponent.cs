using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Components
{
    public class HasHealthComponent : IComponent
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }
}