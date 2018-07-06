using System;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class EntityCollectionTests
    {
        [Fact]
        public void should_create_new_entity()
        {
            var expectedId = Guid.NewGuid();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            var mockEventSystem = Substitute.For<IEventSystem>();
            mockEntityFactory.Create(null).Returns(new Entity(expectedId, mockEventSystem));
       
            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();

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

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();

            mockEventSystem.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.EntityCollection == entityCollection));
        }

        [Fact]
        public void should_raise_event_before_entity_added_to_collection_when_created()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            mockEventSystem
                .When(x => x.Publish(Arg.Any<EntityBeforeAddedEvent>()))
                .Do(x =>
                {
                    var eventData = x.Arg<EntityBeforeAddedEvent>();
                    Assert.DoesNotContain(eventData.Entity, eventData.EntityCollection);
                });

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();

            mockEventSystem.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.EntityCollection == entityCollection));
        }

        [Fact]
        public void should_raise_event_before_entity_added_to_collection()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            mockEventSystem
                .When(x => x.Publish(Arg.Any<EntityBeforeAddedEvent>()))
                .Do(x =>
                {
                    var eventData = x.Arg<EntityBeforeAddedEvent>();
                    Assert.DoesNotContain(eventData.Entity, eventData.EntityCollection);
                });

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            entityCollection.AddEntity(entity);

            mockEventSystem.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.EntityCollection == entityCollection));
        }

        [Fact]
        public void should_add_created_entity_into_the_collection()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();

            Assert.Equal(1, entityCollection.Count());
            Assert.Equal(entity, entityCollection.First());
        }

        [Fact]
        public void should_raise_events_and_remove_components_when_removing_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();
            entityCollection.RemoveEntity(entity.Id);
            
            mockEventSystem.Received().Publish(Arg.Is<EntityRemovedEvent>(x => x.Entity == entity && x.EntityCollection == entityCollection));

            Assert.Empty(entity.Components);
        }

        [Fact]
        public void should_raise_events_before_components_removed_when_removing_entity()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            mockEventSystem
                .When(x => x.Publish(Arg.Any<EntityBeforeRemovedEvent>()))
                .Do(x =>
                {
                    var eventData = x.Arg<EntityBeforeRemovedEvent>();
                    Assert.Contains(eventData.Entity, eventData.EntityCollection);
                });

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();
            entityCollection.RemoveEntity(entity);

            mockEventSystem.Received().Publish(Arg.Is<EntityBeforeRemovedEvent>(x => x.Entity == entity && x.EntityCollection == entityCollection));

            Assert.Empty(entity.Components);
        }

        [Fact]
        public void should_remove_created_entity_from_the_collection()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockEntityFactory = Substitute.For<IEntityFactory>();
            mockEntityFactory.Create(null).Returns(new Entity(Guid.NewGuid(), mockEventSystem));

            var entityCollection = new EntityCollection("", mockEntityFactory, mockEventSystem);
            var entity = entityCollection.CreateEntity();
            entityCollection.RemoveEntity(entity.Id);

            Assert.Empty(entityCollection);
        }
    }
}