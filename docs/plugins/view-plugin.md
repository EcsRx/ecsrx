# Views Plugin

Most games have the notion of a view, in **Unity** this may be your Scene (GameObjects), in **Monogame** it may be your sprites or 3d objects, in **Godot** it would be your nodes, it could even be text written out to a console app.

Ultimately these view bits rely upon the data within your components but will live outside of the core ECS system, i.e in unity your GameObject exists within the Scene and the EcsRx has no knowledge of this view layer.

A large part of game logic will take place around these view related objects, so to this end the notion of views have been added to try and provide some streamlined and consistent way of interacting with the view layer from within the ECS layer. 

This is important as everyone could come up with their own way of managing the notion of views for an entity, and some people still will anyway, however as this is a foundation to be built upon, as if everyone can agree upon this convention then it makes it easier for people to develop and consume plugins which will interact with views.
 
 So the view conventions provided here adds a sort of view contract that not only you as the developer can adhere to but people writing further plugins for the system can follow allowing there to be some basic standards for view creation and control.

## ViewComponent

So the first part of this convention is a `ViewComponent` which contains the underlying View object and some configuration around the view, such as if the entity should self destruct if the view is destroyed.

So to make use of `ViewResolverSystem`s you will need to have applied a `ViewComponent` to you entity, like so:

```csharp
someEntity.AddComponent<ViewComponent>();
```

This will get populated elsewhere via a specific type of system, but when you are wanting to access the view from the ECS layer you will be wanting to get it via this component.

## ViewResolverSystem

This is the other part of the puzzle, and is a specific kind of abstract `ISetupSystem` implementation which specifically targets entities with views and attempts to resolve the view layer object for the `ViewComponent` to use.

The `ViewResolverSystem` and `PooledViewResolverSystem` are both abstract and implement `IViewResolverSystem` and require you to implement your own versions, but the idea is you get provided an entry point to setup your view within your given framework, i.e within Unity it will create prefabs/game objects and apply monobehaviours linking back to the ECS layer.

It is also recommended that you override the `Group` to indicate the grouping for your view. So for example if you were to have 2 view resolvers, one for a vehicle and a player you would want to make sure that you target the right groups for each resolver, here is an example of two hypothetical resolvers for this scenario:

### Example

```csharp
public class VehicleViewResolver : ViewResolverSystem
{
	public override IGroup Group` => base.Group.WithComponent<VehicleComponent>();
	
	public override IViewHandler ViewHandler { get; } = new VehicleViewHandler();
	
	protected override void OnViewCreated(IEntity entity, ViewComponent viewComponent);
	{
		// do something with vehicle
	}
}
```

## Putting it all together

So as long as you apply the `ViewComponent` and add the various resolvers to the `SystemExecutor` it will automatically setup these views for you when the entities are created. It is recommended that you pair this with the notion of blueprints so you can have consistent setups for your entities.

## Do I need this?

This is only really for entities which need visual embodiments in the unity scene, if you are working with purely data driven entities then you wont need to apply and of this to those entities. So don't feel you NEED to have view based entities, it just provides you a mechanism to cope with these scenarios in a consistent way.

## Ignoring all of this and still using game objects

There is nothing stopping you ignoring this whole convention and having your own convention or just having various other components which have game objects and manually creating them via systems or whatever notion you want. This approach is still using entities, components and systems so it is still true to the underlying pattern, it just simplifies the creation and management of views to the core parts, so really think if what you need is not already solved by this approach.