using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ObservableGroupTests
    {
        [Fact]
        public void should_include_entity_snapshot_on_creation()
        {
            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
            var accessorToken = new ObservableGroupToken(new[]{1}, new int[0], "default");

            var applicableEntity1 = Substitute.For<IEntity>();
            var applicableEntity2 = Substitute.For<IEntity>();
            var notApplicableEntity1 = Substitute.For<IEntity>();

            applicableEntity1.Id.Returns(1);
            applicableEntity2.Id.Returns(2);
            notApplicableEntity1.Id.Returns(3);
            
            applicableEntity1.HasComponent(Arg.Any<int>()).Returns(true);
            applicableEntity2.HasComponent(Arg.Any<int>()).Returns(true);
            notApplicableEntity1.HasComponent(Arg.Any<int>()).Returns(false);            
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                applicableEntity1,
                applicableEntity2,
                notApplicableEntity1
            };
            
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var observableGroup = new ObservableGroup(accessorToken, dummyEntitySnapshot, mockCollectionNotifier);

            Assert.Equal(2, observableGroup.CachedEntities.Count);
            Assert.Contains(applicableEntity1, observableGroup.CachedEntities.Values);
            Assert.Contains(applicableEntity2, observableGroup.CachedEntities.Values);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_added()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { 1,2 }, new int[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var unapplicableEntity = Substitute.For<IEntity>();
            unapplicableEntity.Id.Returns(2);
            unapplicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.RequiredComponents.Contains(x))).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var entityAddedSub = new Subject<CollectionEntityEvent>();
            mockCollectionNotifier.EntityAdded.Returns(entityAddedSub);
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new IEntity[0], mockCollectionNotifier);
            var wasCalled = 0;
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled++);
            
            entityAddedSub.OnNext(new CollectionEntityEvent(unapplicableEntity, mockCollection));
            Assert.Empty(observableGroup.CachedEntities);
            
            entityAddedSub.OnNext(new CollectionEntityEvent(applicableEntity, mockCollection));
            Assert.Equal(1, observableGroup.CachedEntities.Count);
            Assert.Equal(applicableEntity, observableGroup.CachedEntities.GetByKey(applicableEntity.Id));
            
            Assert.Equal(1, wasCalled);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_components_match_group()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { 1 }, new []{ 2 }, collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var componentRemoved = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(componentRemoved);
            
            var observableGroup = new ObservableGroup(accessorToken, new IEntity[0], mockCollectionNotifier);
            var wasCalled = 0;
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled++);

            applicableEntity.HasAllComponents(accessorToken.LookupGroup.RequiredComponents).Returns(true);
            applicableEntity.HasAnyComponents(accessorToken.LookupGroup.ExcludedComponents).Returns(false);
            componentRemoved.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, null));
            
            Assert.Contains(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.Equal(1, wasCalled);
        }

        [Fact]
        public void should_remove_entity_and_raise_events_when_entity_removed_with_components()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { 1, 2 }, new int[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.RequiredComponents.Contains(x))).Returns(true);
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.ExcludedComponents.Contains(x))).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var entityRemoved = new Subject<CollectionEntityEvent>();
            var componentRemoving = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityRemoved.Returns(entityRemoved);
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(componentRemoving);
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new[]{applicableEntity}, mockCollectionNotifier);
            var wasRemovingCalled = 0;
            observableGroup.OnEntityRemoving.Subscribe(x => wasRemovingCalled++);            
            var wasRemovedCalled = 0;
            observableGroup.OnEntityRemoved.Subscribe(x => wasRemovedCalled++);
            
            componentRemoving.OnNext(new ComponentsChangedEvent(null, applicableEntity, new[]{1}));
            
            Assert.Contains(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.Equal(1, wasRemovingCalled);
            Assert.Equal(0, wasRemovedCalled);

            wasRemovingCalled = wasRemovedCalled = 0;
            entityRemoved.OnNext(new CollectionEntityEvent(applicableEntity, null));
            
            Assert.DoesNotContain(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.Equal(0, wasRemovingCalled);
            Assert.Equal(1, wasRemovedCalled);
        }
        
        [Fact]
        public void should_remove_entity_and_raise_event_when_no_longer_matches_group()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { 1,2 }, new int[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.RequiredComponents.Contains(x))).Returns(true);
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.ExcludedComponents.Contains(x))).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var componentRemoving = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityComponentsRemoving.Returns(componentRemoving);
            
            var componentRemoved = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityComponentsRemoved.Returns(componentRemoved);
            
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new[]{applicableEntity}, mockCollectionNotifier);
            var wasRemovingCalled = 0;
            observableGroup.OnEntityRemoving.Subscribe(x => wasRemovingCalled++);            
            var wasRemovedCalled = 0;
            observableGroup.OnEntityRemoved.Subscribe(x => wasRemovedCalled++);
            
            applicableEntity.HasAnyComponents(accessorToken.LookupGroup.RequiredComponents).Returns(false);
            applicableEntity.HasAllComponents(accessorToken.LookupGroup.RequiredComponents).Returns(false);
            componentRemoving.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, new[]{ 1 }));
            componentRemoved.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, new[]{ 1 }));
            
            Assert.DoesNotContain(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.Equal(1, wasRemovingCalled);
            Assert.Equal(1, wasRemovedCalled);
        }
        
    }
}