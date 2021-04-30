# Breaking Changes

## 3.12.0 -> 4.0.0

- `IEcsRxPlugin` has been renamed to `ISystemsRxPlugin` and lives in `SystemsRx.Infrastructure`
- `EventReactionSystem<T>` no longer exists, the same convention can be mapped to `IReactToEventSystem` from `SystemsRx`
- `IBasicSystem` has changed and has no `IEntity` dependencies and lives in `SystemsRx`, the same convention can be mapped to `IBasicEntitySystem` in `EcsRx`
- `IManualSystem` no longer has a group or gets passed `IObservableGroupManager`, you can inject it yourself if you need it
- `EcsRx` now depends upon `SystemsRx`, all the classes in `SystemsRx` were originally in `EcsRx` but now can be used without the `ECS` related paradigm dependencies
- `ISystem` no longer contains an `IGroup` and now lives in `SystemsRx` there is now an `IGroupSystem` which represents a system with a `IGroup`

## 3.10.0 -> 3.11.0

- `IEntityCollectionManager` no longer exists, it is now just `IObservableGroupManager`

## 3.9.0 -> 3.10.0

- `IEntityCollectionManager` no longer contains EntityCollections its now within `IEntityDatabase`, which is within there

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
