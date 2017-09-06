using System;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class PoolTests
    {
        [Test]
        public void should_create_new_entity()
        {
            var expectedId = Guid.NewGuid();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEventSystem = Substitute.For<IEventSystem>();
            mockEntityFactory.Create(null).Returns(new Entity(expectedId, mockEventSystem));
       
            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            Assert.That(entity.Id, Is.EqualTo(expectedId));
            Assert.That(entity.Components, Is.Not.Null);
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void should_raise_event_when_creating_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            mockEventSystem.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.Pool == pool));
        }

        [Test]
        public void should_add_created_entity_into_the_pool()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();

            Assert.That(pool.Entities.Count(), Is.EqualTo(1));
            Assert.That(pool.Entities.First(), Is.EqualTo(entity));
        }

        [Test]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);
            
            mockEventSystem.Received().Publish(Arg.Is<EntityRemovedEvent>(x => x.Entity == entity && x.Pool == pool));

            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void should_remove_created_entity_from_the_pool()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var pool = new Pool("", mockEntityFactory, mockEventSystem);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);

            Assert.That(pool.Entities, Is.Empty);
        }
    }
}