using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Examples.ExampleApps.Playground.Batches
{
    public struct CustomClassBatch : IBatchDescriptor
    {
        public int EntityId { get; set; }
        public ClassComponent Basic;
        public ClassComponent2 Basic2;
    }
}