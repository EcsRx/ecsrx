# Component Type Lookups

Out the box most of your component interactions will be done via generics like `entity.GetComponent<SomeComponent>();` and this is fine for most cases.

Behind the scenes when EcsRx starts it builds an index/id list based off the available components and will under the hood use these indexes to interact with the component data, so when you provide it a generic at the entity layer, its actually being looked up and converted into an index to be used in the underlying data stores.

So if you are happy to provide a type index/id with your call it will bypass the lookup step for the generic, meaning less overhead in entity/component interactions, for example the call above would now be `entity.GetComponent<SomeComponent>(ComponentLookupTypes.SomeComponentId)` (or if you dont care about the strong type) `entity.GetComponent(ComponentLookupTypes.SomeComponentId)`.

So this change means **YOU** have to tell EcsRx ahead of time what indexes/ids to use for your components and provide that data to the rest of your codebase, its easiest to do this by making a class with static int properties like so:

```csharp
public static class ComponentLookupTypes
{
    public static int NameComponentId = 0;
    public static int PositionComponentId = 1;
    public static int MovementSpeedComponentId = 2;
}
```

Then you can just reference these types anywhere which satisfies the telling of the entity the index, and you then just need to make sure when the application is created it uses these explicit lookups rather than auto generating them, generally done by making your own module and loading it like so:

```csharp
public class CustomComponentLookupsModule : IDependencyModule
{
    public void Setup(IDependencyContainer container)
    {
        container.Unbind<IComponentTypeLookup>();
        var explicitTypeLookups = new Dictionary<Type, int>
        {
            {typeof(NameComponent), ComponentLookupTypes.NameComponentId},
            {typeof(PositionComponent), ComponentLookupTypes.PositionComponentId},
            {typeof(MovementSpeedComponent), ComponentLookupTypes.MovementSpeedComponentId}
        };
        var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);
        container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});
    }
}
```

This will unbind the default implementation which auto generates, and replaces it with explicit config.

## How much of a performance boost do I get?

**Small performance boost**

Its not a MASSIVE performance boost, but it will reduce lookups into dictionaries on entity interactions, so this can reduce memory consumption in a few scenarios as there is no need for the system to build up `Type` objects etc.