using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Reactive;
using EcsRx.Tests.Models;
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
            var accessorToken = new ObservableGroupToken(new Type[0], new Type[0], "default");

            var applicableEntity1 = Substitute.For<IEntity>();
            var applicableEntity2 = Substitute.For<IEntity>();
            var notApplicableEntity1 = Substitute.For<IEntity>();

            applicableEntity1.Id.Returns(1);
            applicableEntity2.Id.Returns(2);
            notApplicableEntity1.Id.Returns(3);
            
            applicableEntity1.HasAllComponents(Arg.Any<Type[]>()).Returns(true);
            applicableEntity2.HasAllComponents(Arg.Any<Type[]>()).Returns(true);
            notApplicableEntity1.HasAllComponents(Arg.Any<Type[]>()).Returns(false);
            
            applicableEntity1.HasAnyComponents(Arg.Any<Type[]>()).Returns(false);
            applicableEntity2.HasAnyComponents(Arg.Any<Type[]>()).Returns(false);
            notApplicableEntity1.HasAnyComponents(Arg.Any<Type[]>()).Returns(false);
            
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
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(true);

            var unapplicableEntity = Substitute.For<IEntity>();
            unapplicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var entityAddedSub = new Subject<CollectionEntityEvent>();
            mockCollectionNotifier.EntityAdded.Returns(entityAddedSub);
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new IEntity[0], mockCollectionNotifier);
            var wasCalled = false;
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled = true);
            
            entityAddedSub.OnNext(new CollectionEntityEvent(unapplicableEntity, mockCollection));
            Assert.Empty(observableGroup.CachedEntities);
            
            entityAddedSub.OnNext(new CollectionEntityEvent(applicableEntity, mockCollection));
            Assert.Equal(1, observableGroup.CachedEntities.Count);
            Assert.Equal(applicableEntity, observableGroup.CachedEntities[applicableEntity.Id]);
            
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_components_match_group()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne) }, new []{typeof(TestComponentTwo)}, collectionName);
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
            var wasCalled = false;
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled = true);

            applicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(true);
            applicableEntity.HasAnyComponents(accessorToken.Group.ExcludedComponents).Returns(false);
            componentRemoved.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, null));
            
            Assert.Contains(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_remove_entity_and_raise_event_when_entity_removed()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(true);
            applicableEntity.HasAnyComponents(accessorToken.Group.ExcludedComponents).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var entityRemoved = new Subject<CollectionEntityEvent>();
            mockCollectionNotifier.EntityRemoved.Returns(entityRemoved);
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new[]{applicableEntity}, mockCollectionNotifier);
            var wasRemovingCalled = false;
            observableGroup.OnEntityRemoving.Subscribe(x => wasRemovingCalled = true);            
            var wasRemovedCalled = false;
            observableGroup.OnEntityRemoved.Subscribe(x => wasRemovedCalled = true);
            
            entityRemoved.OnNext(new CollectionEntityEvent(applicableEntity, null));
            
            Assert.DoesNotContain(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.True(wasRemovingCalled);
            Assert.True(wasRemovedCalled);
        }

        [Fact]
        public void should_remove_entity_and_raise_event_when_no_longer_matches_group()
        {
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(true);
            applicableEntity.HasAnyComponents(accessorToken.Group.ExcludedComponents).Returns(false);

            var mockCollectionNotifier = Substitute.For<INotifyingEntityCollection>();
    
            var componentRemoving = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityComponentsRemoving.Returns(componentRemoving);
            
            var componentRemoved = new Subject<ComponentsChangedEvent>();
            mockCollectionNotifier.EntityComponentsRemoved.Returns(componentRemoved);
            
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var observableGroup = new ObservableGroup(accessorToken, new[]{applicableEntity}, mockCollectionNotifier);
            var wasRemovingCalled = false;
            observableGroup.OnEntityRemoving.Subscribe(x => wasRemovingCalled = true);            
            var wasRemovedCalled = false;
            observableGroup.OnEntityRemoved.Subscribe(x => wasRemovedCalled = true);
            
            applicableEntity.HasAnyComponents(accessorToken.Group.RequiredComponents).Returns(false);
            applicableEntity.HasAllComponents(accessorToken.Group.RequiredComponents).Returns(false);
            componentRemoving.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, new IComponent[]{ new TestComponentOne() }));
            componentRemoved.OnNext(new ComponentsChangedEvent(mockCollection, applicableEntity, new IComponent[]{ new TestComponentOne() }));
            
            Assert.DoesNotContain(applicableEntity, observableGroup.CachedEntities.Values);
            Assert.True(wasRemovingCalled);
            Assert.True(wasRemovedCalled);
        }
        
    }
}