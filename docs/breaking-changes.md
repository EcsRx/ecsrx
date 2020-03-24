# Breaking Changes

## 3.9.0 -> 3.10.0

- IEntityCollectionManager no longer contains EntityCollections its now within `IEntityDatabase`, which is within there

## 3.8.0 -> 3.9.0

 - `IObservableScheduler` is now known as `IUpdateScheduler` and uses an `ElapsedTime` object not `TimeSpan`

## 3.0.0 -> 3.8.0

- `Group` no longer contains Predicate and has been split into `GroupWithPredicate` object
- `IObservableScheduler` is now inside `EcsRx` core and not infrastructure

## 2.0.0 -> 3.0.0

- Some extension methods around groups have been removed
- Reactive Systems moved to plugin
- Views moved to plugin
- Computeds moved to plugin

## 1.0.0 -> 2.0.0

- Lifecycle changes in application
- Dependency injection contract changes

## 0.*.* -> 1.0.0

### Groups & Collections
- `IGroupAccessor` has become `IObservableGroup`
- `IGroupAccessorFilter`, `ICacheableGroupAccessorFilter`, `IGroupWatcher` has become `IComputedGroup`
- `IPool` has become `IEntityCollection`
- `IPool.Entities` has been removed, `IEntityCollection` is now `IEnumerable<IEntity>`
- `IPoolManager` has become `IEntityCollectionManager`
- `IGroup.TargettedComponents` has become `IGroup.RequiredComponents`
- CRUD operations on `IPool/Manager` no longer raise system wide events, they are now local `IObservable` events
- CRUD events are now batched, i.e `ComponentAddedEvent` has become `ComponentsAddedEvent`

### Systems
- All system `Execute` methods have been renamed to `Process`
- `ITeardownSystem` is now triggered JUST BEFORE an entity has required components removed
- All systems (other than `IManualSystem`) have been moved to a separate `EcsRx.System` project

### Entities
- `IEntity.Id` is no longer a `Guid` and is now an `int`
- `IEntity.AddComponent<T>` has been removed, but kept as an extension method on `IEntity`
- Most interactions at entity level are batched, i.e `AddComponent<T>()` is now `AddComponents(params IComponent[] components)`
