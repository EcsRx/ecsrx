using System;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class PoolTests
    {
        [Fact]
        public void should_create_new_entity()
        {
            var expectedId = Guid.NewGuid();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEventSystem = Substitute.For<IEventSystem>();
            mockEntityFactory.Create(null).Returns(new Entity(expectedId, mockEventSystem));
       
            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            Assert.Equal(expectedId, entity.Id);
            Assert.NotNull(entity.Components);
            Assert.Empty(entity.Components);
        }

        [Fact]
        public void should_raise_event_when_creating_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            mockEventSystem.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.Pool == pool));
        }

        [Fact]
        public void should_add_created_entity_into_the_pool()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            Assert.Equal(1, pool.Entities.Count());
            Assert.Equal(entity, pool.Entities.First());
        }

        [Fact]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);
            
            mockEventSystem.Received().Publish(Arg.Is<EntityRemovedEvent>(x => x.Entity == entity && x.Pool == pool));

            Assert.Empty(entity.Components);
        }

        [Fact]
        public void should_remove_created_entity_from_the_pool()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);

            Assert.Empty(pool.Entities);
        }
    }
}