# Observable Groups

So now you know what entities are and how you can get hold of them, its worth going over how observable groups work and how the flow of entities goes through the eco-system.

## Filtration Flow

```
IEntityCollectionManager	<-  This contains all pools, which in turn contains ALL entities
     |
     |
IObservableGroup      		<-  This filters all entities down to only ones which are within the group
     |                    		i.e All entities which contain PlayerComponent
     |
IComputedGroup        		<-  This acts as another layer of filtration on an IObservableGroup
                          		i.e Top 5 entities with PlayerComponent sorted by Score
```

## IEntityCollectionManager

The entity collection manager is the root most point where all entity queries should originate from.

The entity collection manager also maintains a collection of `IObservableGroup` so if you have 5 systems which all use the same group, there will only actually be 1 instance of the `IObservableGroup` that is shared between them all. 

## IObservableGroup

The observable group is created within and maintained by an `IEntityCollectionManager` and exposes all entities which match the associated group. It also exposes observables to represent when an entity has been added or removed from the underlying group.

Under the hood, the observable group watches entities to see when they are added/removed from pools as well as when their components change. When this occurs it will check to see if it effects the current group and if so it updates it internal list accordingly.

This has a huge performance benefit as it will stop you needing to evaluate linq chains into the underlying pools and just give you a cached list of all entities currently matching, given also that these observable groups are shared between any systems that require the same underlying components it can save a lot of resources.

## IComputedGroup

The computed group is created manually and requires an `IObservableGroup` for it to use for the basis of its queries. It is provided to allow you to filter past the group level and get more specific data sets without having to hardcode the logic for the lookup in various systems.

General use cases for this may be things like:

- Get 5 highest scoring players
- Get enemies within a radius of the player
- Transform entitys with various game state components to some single poco for saving state

It is meant to be an interface for you to implement with your own filtration logic, however there is an existing abstract implementation which provides caching for the filter results out of the box.

### `CacheableComputedGroup`

This group filter has caching built in so it will try to keep a pre-evaluted list of entities which match the filtration requirements, this can be beneficial if you are using this in a few places and want it to update automatically when the underlying data changes.

## Querys

So up to this point we have discussed the general filtration process, however there are some extension methods which let you do more ad-hoc queries on data, these are not cached in any way but allow you to drill down into a subset of data in a pre-defined way, this overlaps a bit with the `IComputedGroup` but lets you query directly at the pool level or accessor level.

Both `IEntityCollection` and `IObservableGroup` has a `Query` extension method which takes an `IEntityCollectionQuery` or `IObservableGroupQuery` where you can implement your desired query logic. This was added so you could use the pools and group accessors more like repositories and use pre defined queries to access them consistently.