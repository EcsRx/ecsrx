using System;
using System.Collections.Generic;
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
        public void should_create_default_collection_when_running_constructor()
        {
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            var defaultCollection = Assert.Single(collectionLookup);
            Assert.Equal(EntityCollectionLookups.DefaultCollectionId, defaultCollection.Id);
        }
        
        [Fact]
        public void should_add_collection_and_raise_event()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            var wasCalled = false;
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            
            entityDatabase.CollectionAdded.Subscribe(x => wasCalled = true);
            entityDatabase.AddCollection(expectedEntityCollection);
            
            Assert.Contains(expectedEntityCollection, entityDatabase.Collections);
            Assert.True(wasCalled);
        }
        
        [Fact]
        public void should_throw_exception_adding_collection_that_already_exists_with_id()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            var wasCalled = false;
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            
            entityDatabase.CollectionAdded.Subscribe(x => wasCalled = true);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(expectedEntityCollection);
            
            entityDatabase.CollectionAdded.Subscribe(x => wasCalled = true);
            
            Assert.Throws<ArgumentException>(() => entityDatabase.AddCollection(expectedEntityCollection));
            Assert.Equal(2, collectionLookup.Count);
            Assert.Contains(expectedEntityCollection, entityDatabase.Collections);
            Assert.False(wasCalled);
        }
        
        [Fact]
        public void should_remove_collection_and_raise_event_when_exists()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            var wasCalled = false;
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(expectedEntityCollection);
            entityDatabase.SubscribeToCollection(expectedEntityCollection);

            entityDatabase.CollectionRemoved.Subscribe(x => wasCalled = true);
            entityDatabase.RemoveCollection(expectedEntityCollection.Id);
            
            Assert.False(collectionLookup.Contains(expectedEntityCollection.Id));
            Assert.True(wasCalled);
        }
        
        [Fact]
        public void should_return_when_removing_collection_that_doesnt_exist()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            var wasCalled = false;
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            entityDatabase.CollectionRemoved.Subscribe(x => wasCalled = true);
            entityDatabase.RemoveCollection(expectedEntityCollection.Id);
            
            Assert.False(collectionLookup.Contains(expectedEntityCollection.Id));
            Assert.False(wasCalled);
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
        
        [Fact]
        public void should_get_entity_from_indexer_when_exists()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            var collectionLookup = (CollectionLookup)entityDatabase.GetType()
                .GetField("_collections", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(entityDatabase);
            
            collectionLookup.Add(expectedEntityCollection);
            
            var actualCollection = entityDatabase[expectedEntityCollection.Id];
            Assert.Equal(expectedEntityCollection, actualCollection);
        }
        
        [Fact]
        public void should_throw_get_entity_from_indexer_when_doesnt_exists()
        {
            var expectedEntityCollection = new EntityCollection(22, null);
            
            var mockEntityCollectionFactory = Substitute.For<IEntityCollectionFactory>();
            var entityDatabase = new EntityDatabase(mockEntityCollectionFactory);
            Assert.Throws<KeyNotFoundException>(() => entityDatabase[expectedEntityCollection.Id]);
        }
    }
}