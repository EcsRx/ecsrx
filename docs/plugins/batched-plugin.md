# Batched Systems Plugin

Batches systems provide a way to pre-fetch and batch process entities within a system. This offers a huge performance boost as it only needs to re-build the batches when the associated group changes so compared to a regular `IReactToGroup` based system this could be MANY times faster as it doesnt have to re-evaluate the entities and components every process it just evaluates them once and caches them and their components in memory and provides them for you to use.

You can use this system with struct based components OR class based components, but out the box there is no implementation which mixes them, but you can make your own custom implementations for these scenarios if needed.

## How do I use it?

Just load the plugin and then extend the required batched system, most of the heavy lifting is done for you behind the scenes.

### Using struct based components

```csharp
public class BatchedMovementSystem : BatchedSystem<PositionComponent, MovementSpeedComponent>
{
    public BatchedMovementSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler)
    {}

    protected override IObservable<bool> ReactWhen()
    { return Observable.Interval(TimeSpan.FromSeconds(0.5f)).Select(x => true); }

    protected override void Process(int entityId, ref PositionComponent positionComponent, ref MovementSpeedComponent movementSpeedComponent)
    {
        positionComponent.Position += Vector3.One * movementSpeedComponent.Speed;
    }
}
```

As you can see we extend the `BatchedSystem` class and provide it the component types as the generic arguments, then in the `Process` method you will get the entity id and then a `ref` to each component type required.

### Using class based components

The setup is almost identical to the struct based one but instead of using `BatchedSystem` you use `ReferenceBatchedSystem` as shown below:

```csharp
public class BatchedMovementSystem : ReferencedBatchedSystem<PositionComponent, MovementSpeedComponent>
{
    public BatchedMovementSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler)
    {}

    protected override IObservable<bool> ReactWhen()
    { return Observable.Interval(TimeSpan.FromSeconds(0.5f)).Select(x => true); }

    protected override void Process(int entityId, PositionComponent positionComponent, MovementSpeedComponent movementSpeedComponent)
    {
        positionComponent.Position += Vector3.One * movementSpeedComponent.Speed;
    }
}
```

There is a minor difference in constructor and your components passed in are the reference types rather than `ref`s to the structs, but this is all that is needed to use batched plugins.