using System.Runtime.InteropServices;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Examples.ExampleApps.Playground.Batches
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CustomStructBatch : IBatchDescriptor
    {
        public int EntityId { get; set; }
        public StructComponent Basic;
        public StructComponent2 Basic2;
    }
}