# Entities

Entities are basically things that exist in your world with components on them, just like lots of little databases. Each entity has a unique ID and a list of components, these can be added and removed whenever you want, and this will cause the entities to fit into different groups.

## Creating entities

So entities are created via entity collections so you don't need to do much here other than get the collection you wish to create your entity in and call the `CreateEntity` method which will return you a method to play with.

## Accessing entities

As mentioned entities are created within collections and you can have many entity collections if you want to segregate entities for whatever reason, and in most cases the systems will get the applicable entities for you, but if you are wanting to find out more about how it all works or wanting to go and get entities yourself for whatever reason you have the following options:

### From `IEntityCollectionManager`

- `myCollectionManager.GetObservableGroup(myGroup, idsForCollectionsToCheck)`

This is probably the most common approach, you get your `IEntityCollectionManager` instance (usually injected in to your class) and you call `CreateObservableGroup`, this will create or return an existing observable group for you which internally contains the `Entities` that match the group for you to query on further. This is often a better approach than accessing entities directly. (read more on this in querying/filtration docs)

- `myCollectionManager.EntityDatabase.*`

The entity collection manager exposes the entity database which can be queried for more info 

### From `IEntityDatabase`

- `entityDatabase.GetEntitiesFor(myGroup, idsForCollectionsToCheck)`

This is not used often but is there for convenience, it allows you to just get back an `IEnumerable<IEntity>` collection which contains all entities which match the group, so it you can query on the matching entities further however you want.

### From `IEntityCollection`

The `IEntityCollection` interface actually implements `IEnumerable` so you can just treat the collection like an enumerable object and foreach/linq query it as you would any other collection object.

## Destroying entities

To destroy an entity, remove it from the IEntityCollection it belongs to.

- `entityCollection.RemoveEntity(entity.Id);`

## Recommendations

It is recommended in most cases to let the systems handle the accessing of entities as the systems have processes in place to manage all this, also `IObservableGroups` are re-used wherever possible meaning less evaluations of data when re-used across systems.

If however you have a reason to need to go and access entities directly outside of systems, or manually within things like `IManualSystem` types, you have an avenue to go down.
