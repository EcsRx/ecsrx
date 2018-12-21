using EcsRx.Groups.Batches;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Batches
{
    public struct TestBatch : IBatchDescriptor
    {
        public int EntityId { get; set; }
        public TestStructComponentOne StructComponentOne;
        public TestComponentOne TestComponentOne;
    }
}