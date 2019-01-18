using System.Runtime.InteropServices;
using EcsRx.Examples.ExampleApps.Playground.Components;

namespace EcsRx.Examples.ExampleApps.Playground.Batches
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CustomStructBatch
    {
        public int EntityId;
        public StructComponent Basic;
        public StructComponent2 Basic2;
    }
}