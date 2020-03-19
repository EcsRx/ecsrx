# Reactive Systems Plugin

This plugin contains some pre-made systems that cover common use cases where you want to use rx streams to govern your system processing.

## ISetupSystem

This interface implies that you want to setup entities, so it will match all entities via the group and will run a `Setup` method once for each of the entities. This is primarily there for doing one off setup methods on entities, such as instantiating `GameObject` or complex object types.

## ITeardownSystem

This is similar to `ISetupSystem`, but is used when a matched entity's group is removed.

## IReactToEntitySystem

This interface implies that you want to react to individual changes in an entity. It will pass each entity to the `ReactToEntity` method to setup the observable you want, such as Health changing, input occurring, random intervals etc. This only happens once per matched entity, here is an example of the sort of thing you would do here:

```csharp
public IObservable<IEntity> ReactToEntity(IEntity entity)
{
    var colorComponent = entity.GetComponent<RandomColorComponent>();
    return colorComponent.Color.DistinctUntilChanged().Select(x => entity);
}
```

Once you have setup your reactions the `Process` method is triggered every time the subscription from the reaction phase is triggered, so this way your system reacts to data rather than polling for changes each frame, this makes system logic for succinct and direct, it also can make quite complex scenarios quite simple as you can use the power of **Rx** to daisy chain together your observables to trigger whatever you want.

It is also worth looking at the groups documentation as there are some features in groups which allow you to automatically constrain entities based upon predicates so that can push some constraining logic up to the *group* rather than your specific system, however sometimes it makes sense to be in the system, so its your call.

## IReactToGroupSystem

This is like the `IReactToEntitySystem` but rather than reacting to each entity matched, it instead just reacts to something at the group level. The `ReactToGroup` is generally used as a way to process all entities every frame using `Observable.EveryUpdate()` and selecting the group, however you can do many other things such as reacting to events at a group level or some other observable notion, here is a simple example:

```csharp
public IObservable<GroupAccessor> ReactToGroup(GroupAccessor group)
{
    return Observable.EveryUpdate().Select(x => group);
}
```

The main benefit of this interface vs the `IReactToEntitySystem` approach is that this one will only generate a single subscription to trigger all entities in the group to be processed, where the other interface would generate a subscription per entity, so this is a much more performant way of reacting to the same thing for the entire group.

## IReactToDataSystem<T>

So this is the more complicated and lesser used flavour of system. It is basically the same as the `IReactToEntitySystem` however it reacts to data rather than an entity. So for example lets say you wanted to react to a collision event and your system wanted to know about the entity as normal, but also the collision event that occurred. This system is the way you would do that, as its subscription passes back some data rather than an entity, here is an example:

```csharp
IObservable<CollisionEvent> ReactToEntity(IEntity entity)
{
    return MessageBroker.Receive<EntityCollisionEvent>().Single(x => x.collidee == entity);
}
```

So this offers a bit more power as the `Process` method takes both the entity in the pool and the returned data from the subscription allowing you to work with external data when processing.

This is still a fairly new system type so will possibly have some minor changes as we move forward.

### EventReactionSystem<T>

This is a custom implementation of the IManualSystem which acts as a shorthand way of waiting for an event then
once it occurs running some logic based purely upon the logic within that event. (However you can override 
it a bit to provide additional entities outside of the group if needed).

```csharp
public class SomeEventSystem : EventReactionSystem<SomeEvent>
{
    public SomeEventSystem(IEventSystem eventSystem) : base(eventSystem)
    { }

    public override void EventTriggered(SomeEvent eventData)
    {
        // Do something with eventData
    }
}
```