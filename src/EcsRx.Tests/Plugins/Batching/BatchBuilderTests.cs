using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Plugins.Batching
{
    public class BatchBuilderTests
    {
        [Fact]
        public unsafe void should_create_batch_with_correct_values()
        {
            var fakeOne1 = new TestStructComponentOne {Data = 10};
            var fakeOne2 = new TestStructComponentOne {Data = 20};
            var fakeOnes = new[] {fakeOne1, fakeOne2};
            
            var fakeTwo1 = new TestStructComponentTwo {Data = 1.5f};
            var fakeTwo2 = new TestStructComponentTwo {Data = 2.6f};
            var fakeTwos = new[] {fakeTwo1, fakeTwo2};
            
            var mockComponentDatabase = Substitute.For<IComponentDatabase>();
            mockComponentDatabase.GetComponents<TestStructComponentOne>(Arg.Any<int>()).Returns(fakeOnes);
            mockComponentDatabase.GetComponents<TestStructComponentTwo>(Arg.Any<int>()).Returns(fakeTwos);
            
            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentType(typeof(TestStructComponentOne)).Returns(0);
            mockTypeLookup.GetComponentType(typeof(TestStructComponentTwo)).Returns(1);

            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.ComponentAllocations.Returns(new[] {0, 0});
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.ComponentAllocations.Returns(new[] {1, 1});
            
            var fakeEntities = new []{ fakeEntity1, fakeEntity2 };
            
            var batchBuilder = new BatchBuilder<TestStructComponentOne, TestStructComponentTwo>(mockComponentDatabase, mockTypeLookup);

            var batches = batchBuilder.Build(fakeEntities);
                
            Assert.Equal(fakeEntities.Length, batches.Length);
            Assert.Equal(fakeEntities[0].Id, batches[0].EntityId);
            Assert.Equal(fakeOne1.Data, (*batches[0].Component1).Data);
            Assert.Equal(fakeTwo1.Data, (*batches[0].Component2).Data);
            Assert.Equal(fakeEntities[1].Id, batches[1].EntityId);
            Assert.Equal(fakeOne2.Data, (*batches[1].Component1).Data);
            Assert.Equal(fakeTwo2.Data, (*batches[1].Component2).Data);
        }
    }
}