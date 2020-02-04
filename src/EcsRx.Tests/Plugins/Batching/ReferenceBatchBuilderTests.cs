using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Plugins.Batching
{
    public class ReferenceBatchBuilderTests
    {
        private class DummyComponentDatabase : IComponentDatabase
        {
            private readonly Dictionary<Type, Array> _components;

            public DummyComponentDatabase(TestComponentOne[] c1, TestComponentTwo[] c2)
            {
                _components = new Dictionary<Type, Array>
                {
                    [typeof(TestComponentOne)] = c1,
                    [typeof(TestComponentTwo)] = c2
                };
            }

            public int Allocate(int componentTypeId)
            {
                throw new NotImplementedException();
            }

            public T Get<T>(int componentTypeId, int allocationIndex) where T : IComponent
            {
                throw new NotImplementedException();
            }

            public Span<T> GetComponents<T>(int componentTypeId) where T : IComponent => (T[])_components[typeof(T)];

            public IComponentPool<T> GetPoolFor<T>(int componentTypeId) where T : IComponent
            {
                throw new NotImplementedException();
            }

            public ref T GetRef<T>(int componentTypeId, int allocationIndex) where T : IComponent
            {
                throw new NotImplementedException();
            }

            public GCHandle Pin<T>(int componentTypeId) where T : IComponent
            {
                throw new NotImplementedException();
            }

            public void PreAllocateComponents(int componentTypeId, int allocationSize)
            {
                throw new NotImplementedException();
            }

            public void Remove(int componentTypeId, int allocationIndex)
            {
                throw new NotImplementedException();
            }

            public void Set<T>(int componentTypeId, int allocationIndex, T component) where T : IComponent
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void should_create_batch_with_correct_values()
        {
            var fakeOne1 = new TestComponentOne { Data = "hello" };
            var fakeOne2 = new TestComponentOne { Data = "hi" };
            var fakeOnes = new[] { fakeOne1, fakeOne2 };

            var fakeTwo1 = new TestComponentTwo { Data = "goodbye" };
            var fakeTwo2 = new TestComponentTwo { Data = "bye" };
            var fakeTwos = new[] { fakeTwo1, fakeTwo2 };

            var mockComponentDatabase = new DummyComponentDatabase(fakeOnes, fakeTwos);

            var mockTypeLookup = Substitute.For<IComponentTypeLookup>();
            mockTypeLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            mockTypeLookup.GetComponentType(typeof(TestComponentTwo)).Returns(1);

            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.ComponentAllocations.Returns(new[] { 0, 0 });

            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.ComponentAllocations.Returns(new[] { 1, 1 });

            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };

            var batchBuilder = new ReferenceBatchBuilder<TestComponentOne, TestComponentTwo>(mockComponentDatabase, mockTypeLookup);

            var batches = batchBuilder.Build(fakeEntities);

            Assert.Equal(fakeEntities.Length, batches.Length);
            Assert.Equal(fakeEntities[0].Id, batches[0].EntityId);
            Assert.Equal(fakeOne1.Data, batches[0].Component1.Data);
            Assert.Equal(fakeTwo1.Data, batches[0].Component2.Data);
            Assert.Equal(fakeEntities[1].Id, batches[1].EntityId);
            Assert.Equal(fakeOne2.Data, batches[1].Component1.Data);
            Assert.Equal(fakeTwo2.Data, batches[1].Component2.Data);
        }
    }
}