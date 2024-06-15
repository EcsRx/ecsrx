using System;
using System.Linq;
using System.Reflection;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Collections.Events;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Lookups;
using NSubstitute;
using R3;
using Xunit;

namespace EcsRx.Tests.EcsRx.Database
{
    public class EntityDatabaseExtensionTests
    {
        [Fact]
        public void should_get_entity_from_collections_when_exists()
        {
            var entityId = 101;
            var expectedEntity = Substitute.For<IEntity>();
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Id.Returns(0);
            mockEntityCollection.ContainsEntity(entityId).Returns(true);
            mockEntityCollection.GetEntity(entityId).Returns(expectedEntity);
            mockEntityCollection.EntityAdded.Returns(new Subject<CollectionEntityEvent>());
            mockEntityCollection.EntityRemoved.Returns(new Subject<CollectionEntityEvent>());
            mockEntityCollection.EntityComponentsAdded.Returns(new Subject<ComponentsChangedEvent>());
            mockEntityCollection.EntityComponentsRemoving.Returns(new Subject<ComponentsChangedEvent>());
            mockEntityCollection.EntityComponentsRemoved.Returns(new Subject<ComponentsChangedEvent>());
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            mockEntityCollectionFactory.Create(Arg.Is<int>(mockEntityCollection.Id)).Returns(mockEntityCollection);
            
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var actualEntity = entityDatabase.GetEntity(entityId);
            Assert.Equal(expectedEntity, actualEntity);
        }
        
        [Fact]
        public void should_get_null_from_collections_when_doesnt_exist()
        {
            var entityId = 101;
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.Id.Returns(0);
            mockEntityCollection.ContainsEntity(entityId).Returns(false);
            mockEntityCollection.EntityAdded.Returns(new Subject<CollectionEntityEvent>());
            mockEntityCollection.EntityRemoved.Returns(new Subject<CollectionEntityEvent>());
            mockEntityCollection.EntityComponentsAdded.Returns(new Subject<ComponentsChangedEvent>());
            mockEntityCollection.EntityComponentsRemoving.Returns(new Subject<ComponentsChangedEvent>());
            mockEntityCollection.EntityComponentsRemoved.Returns(new Subject<ComponentsChangedEvent>());
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            mockEntityCollectionFactory.Create(Arg.Is<int>(mockEntityCollection.Id)).Returns(mockEntityCollection);

            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            
            var actualEntity = entityDatabase.GetEntity(entityId);
            Assert.Null(actualEntity);
        }
    }
}