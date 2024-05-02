# Blueprints

Blueprints provide a re-useale way to setup entities with pre-defined components and values, it is ultimately up to the developer 
how they wish to implement blueprints, as you can add as much configurable properties to it as you wish but they all implement an 
`Apply` method which takes the entity and sets it up.

Here is an example of a blueprint:

```csharp
public class PlayerBlueprint : IBlueprint
{
	public string Name {get;set;}
	public string Class {get;set;}
	public int Health {get;set;}
	
	public void Apply(IEntity entity)
	{
		entity.AddComponent(new HasNameComponent{ Name = Name });
		entity.AddComponent(new HasClassComponent{ Class = Class });
		entity.AddComponent(new HasHealthComponent{ MaxHealth = Health, CurrentHealth = Health });
	}
}
```

## Creating via blueprints

Pools are aware of blueprints and you can create an entity with a blueprint to save you the time of having to create the entity 
then manually applying all the components, which would look like:

```csharp
var hanSoloEntity = somePool.createEntity(new PlayerBlueprint{ 
	Name = "Han Solo", 
	Class = "Smuggler", 
	Health = 100});
```

## Applying blueprints to existing entities

You have 2 options of applying blueprints to entities, one would be to just new up the desired blueprint and call the 
`Apply` method passing in the entity, or you could use the available extension methods to apply directly from the entity, 
this is also chainable so you are able to apply multiple blueprints to the same entity if you wanted, like so:

```csharp
entity.ApplyBlueprint(new DefaultActorBlueprint())
	.ApplyBlueprint(new DefaultEquipmentBlueprint())
	.ApplyBlueprint(new SetupNetworkingBlueprint());
```


## How much should a blueprint do?

This is ultimately up to you, but it was conceived as a way to do bulk component assignments to entities with a small 
amount of config or logic setup. Anything more than that should probably be handled by `SetupSystems` or some other object.

For example a blueprint ideally shouldnt need to have anything injected into it, so if you need to setup complex data 
like setting up a sprite or texture, or some other data payload from a 3rd party source, its recommended you let the blueprint
add the component to the entity, then have some system that catches that entity once its got the component added and then
have the system provide the 3rd party dependency data and set it up (i.e a `SetupSystem`).

## Blurb

Generally you would just use a single blueprint to setup an object, and currently this can only be done in code, however 
now there is the notion of views it should be possible to expose blueprints to the editor in some meaningful way.
