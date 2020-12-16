using System.Numerics;
using System.Runtime.InteropServices;
using EcsRx.Components;

namespace EcsRx.Examples.ExampleApps.Playground.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StructComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public float Something { get; set; }
    }
}