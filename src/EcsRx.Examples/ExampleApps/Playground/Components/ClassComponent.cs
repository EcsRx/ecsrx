using System.Numerics;
using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.Playground.Components
{
    public class ClassComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public float Something { get; set; }
    }
}