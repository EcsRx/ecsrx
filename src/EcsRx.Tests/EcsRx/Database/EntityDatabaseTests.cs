using System.Reflection;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Lookups;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Database
{
    public class EntityDatabaseTests
    {
        [Fact]
        public void should_add_collection_into_existing_collections()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            
            entityDatabase.AddCollection(expectedEntityCollection);
            Assert.Contains(expectedEntityCollection, entityDatabase.Collections);
        }
        
        [Fact]
        public void should_remove_collection_when_exists()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(expectedEntityCollection);
            entityDatabase.SubscribeToCollection(expectedEntityCollection);

            entityDatabase.RemoveCollection(expectedEntityCollection.Id);
            
            Assert.False(collectionLookup.Contains(expectedEntityCollection.Id));
        }
        
        [Fact]
        public void should_get_collection_when_exists()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(expectedEntityCollection);
            
            var actualCollection = entityDatabase.GetCollection(expectedEntityCollection.Id);
            Assert.Equal(expectedEntityCollection, actualCollection);
        }
        
        [Fact]
        public void should_get_null_when_collection_doesnt_exist()
        {
            var missingEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var actualCollection = entityDatabase.GetCollection(missingEntityCollection.Id);
            Assert.Null(actualCollection);
        }
    }
}