using System;
using System.Linq;
using System.Reflection;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Lookups;
using NSubstitute;
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
            mockEntityCollection.ContainsEntity(entityId).Returns(true);
            mockEntityCollection.GetEntity(entityId).Returns(expectedEntity);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(mockEntityCollection);
            
            var actualEntity = entityDatabase.GetEntity(entityId);
            Assert.Equal(expectedEntity, actualEntity);
        }
        
        [Fact]
        public void should_get_null_from_collections_when_doesnt_exist()
        {
            var entityId = 101;
            var mockEntityCollection = Substitute.For<IEntityCollection>();
            mockEntityCollection.ContainsEntity(entityId).Returns(false);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(mockEntityCollection);
            
            var actualEntity = entityDatabase.GetEntity(entityId);
            Assert.Null(actualEntity);
        }
    }
}