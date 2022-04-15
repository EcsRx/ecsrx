# Groups

To access entities from entity collection we have the notion of groups (implementations of `IGroup`). As every *system* requires a group to access entities they are quite an important piece of the puzzle.

So for example lets say that I wanted the notion of a `Player`, that may actually be expressed as an entity with `IsPlayerControlled`, `IsSceneActor`, `HasStatistics` components. If we were to pretend that `IsPlayerControlled` means that the player controls this entity, the `IsSceneActor` implies that you have a `GameObject` in the scene which does some random stuff, and `HasStatistics` which contains information like Health, Mana, Strength etc. Now if we assume a `Player` group is expressed with those, you could express an `NPC` as an entity with just `IsSceneActor` and `HasStatistics`, so this way you can look at your entities in a high level way but making the best use of your more granular components.

## How groups work

So groups are pretty simple, they are just POCOs which describe the component types that you wish to match from within the collection of entities. So if you have hundreds of entities all with different components you can use a group as a way of expressing a high level intent for a system. 

So *Entity Collections* expose a way to pass a group in and get entities matching that group back, as mentioned the primary matching mechanism is via component but there is also the notion of entity matching, which is more experimental, and you can find out more about these further down this page.

## Creating Groups

There are a few different ways to create a group, here are some of the common ways.

### Instantiate a group

There is a `Group` class which implements `IGroup`, this can be instantiated and passed any of the components you want to target, like so:

```csharp
var group = new Group(typeof(SomeComponent));
```

There are also some helper methods here so you can add component types if needed via extension methods, like so:

```csharp
var group = new Group()
    .WithComponent<SomeComponent>()
    .WithoutComponent<SomeOtherComponent();
```

This is a halfway house between the builder approach and the instantiation approach, which is often handy if you want to quickly create a new group from an existing one. It also supports adding many components at once via params if needed.

### Use the GroupBuilder

So there is also a `GroupBuilder` class which can simplify making complex groups, it is easy to use and allows you to express complex group setups in a fluent manner, like so:

```csharp
var group = new GroupBuilder()
    .WithComponent<SomeComponent>()
    .WithComponent<SomeOtherComponent>()
    .Build();
```

### Implement your own IGroup

So if you are going to be using the same groupings a lot, it would probably make sense to make your own implementation of `IGroup`, this will mean less faffing with the above concepts to build a group.

It is quite simple to make your own group, you just need to implement the 2 getters:

```csharp
public class MyGroup : IGroup
{
    public IEnumerable<Type> RequiredComponents {get;} =  return new[] { typeof(SomeComponent), typeof(SomeOtherComponent) };
        
    public IEnumerable<Type> ExcludedComponents {get;} =  return new[] { typeof(SomeComponentIDontWant) };
}
```

As you can see, you can now just instantiate `new MyGroup();` and everyone is happy.

## Required/Excluded Components and TargettedEntities

So as mentioned most entities and constrained by their component types, this is the `RequiredComponents` and `ExcludedComponents` parts, however there is also a `TargettedEntities` notion, which basically takes the constraining a step further and allows you to constrain further on the entities matching the components.

When a group also implements `IHasPredicate` an additional method `CanProcessEntity` can determine if an entity should be matched or not.
For ease of use just use the `GroupWithPredicate` class instead of `Group` and specify an additional `entityPredicate`. Currently these are checked before subscriptions are passed to the system to execute, so you can express some complex constraints without having to be specific in each system.

So for example lets say you wanted a system which would only react to characters with `health == 0`, you could have the component based grouping and then in your system reaction or execution phase check to see if the health == 0 and if not return, however if you had 2 systems wanting to react to the notion of a character death you would end up duplicating your code a lot.

So this is where this functionality pays dividends as you can express this collation at a group level, so you can then make sure your systems will not even execute on the entities unless the predicate is matched.

### Warning

Currently this was implemented as it gives a nicer way to express high level grouping logic, however there may be some niche use-cases where this causes more confusion than it should, so this feature may be removed or changed slightly going forward as there is computed groups which has some overlap with this sort of functionality.