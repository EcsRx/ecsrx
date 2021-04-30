# Systems

Systems are where all the logic lives, it takes entities from the collection and executes logic on each one. The way systems are designed there is an orchestration layer which wraps all systems and handles the communication between the pools and the execution/reaction/setup methods known as `ISystemExecutor` (Which can be read about on other pages).

This means your systems don't need to worry about the logistics of getting entities and dealing with them, you just express how you want to interact with entities and let the `SystemExecutor` handle the heavy lifting and pass you the entities for processing. This can easily be seen when you look at all the available system interfaces which all process individual entities not groups of them.

> Since version 4.0.0 of EcsRx some systems can be used without the Ecs paradigm and live within the `SystemsRx` library, which can be included without the need for `EcsRx`. 

## System Types

This is where it gets interesting, so we have multiple flavours of systems depending on how you want to consume the entities, by default there is `IManualSystem` but there is a project containing all most common systems (`EcsRx.Systems`). You can also mix them up so you could have a single system implement `ISetupSystem`, `ITeardown` and `IReactToEntitySystem` which would run a setup method for each entity when it joins the group then react to the entity changes and process them on changes, then finally run some logic when the entity is being removed from the group.

All **ECS** *(Not SystemsRx)* systems have the notion of a `Group` which describes what entities to target out of the pool, so you don't need to do much other than setup the right groupings and implement the methods for the interfaces.

### `IManualSystem` *(SystemsRx)*

This is a niche system for when you want to carry out some logic outside the scope of entities, or want to have 
more fine grained control over how you deal with the entities matched.

Rather than the `SystemExecutor` doing most of the work for you and managing the subscriptions it leaves it up to you
to manage everything how you want once the system has been started.

The `StartSystem` method will be triggered when the system has been added to the executor, and the `StopSystem` 
will be triggered when the system is removed.

### `IBasicSystem` *(SystemsRx)*

This is a basic system that is triggered every update (based on scheduler update frequency) and lets you do anything you want per update.

### `IReactToEventSystem` *(SystemsRx)*

This allows you to react to any event of that type which is published over the `IEventSystem`.

### `IBasicEntitySystem` *(EcsRx)*

This system allows you to process each entity within the group on every update cycle (much like `IBasicSystem` but with entities).

### `ReactiveSystems`

There is a few plugins that contain more contextual systems that can be read about more in the docs.

## System Loading Order

So by default (with the default implementation of `ISystemExecutor`) systems will load in the order of:

1. Implementations of `ISetupSystem`
2. Implementations of `IReactToEntitySystem`
3. Other Systems 

However within those groupings it will load the systems in whatever order Zenject/Extenject (assuming you are using it) provides them, however there is a way to enforce some level of priority by applying the `[Priority(1)]` attribute, this allows you to specify the priority of how systems should be loaded. The ordering will be from lowest to highest so if you have a priority of 1 it will load before a system with a priority of 10.
