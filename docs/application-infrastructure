# Infrastructure

As part of EcsRx there is some basic infrastructure provided for you (if you choose to use `EcsRx.Infrastructure`), this contains:

- A dependency injection abstraction system (So you can consume DI on any platform with any DI framework)
- An `IEcsRxApplication` interface as well as a default implementation `EcsRxApplication` (So you can start your app in a consistent way)
- A plugin framework via `IEcsRxPlugin` (so you can write your own plugins which can be re-used accross many projects and shared with others)
- A default `EventSystem` (So you can send events around your application, which implements `IEventSystem`)

All of this combined basically provides you an entry point to start creating your applications.

## Application Lifecycle

The default application class should be inherited from and overidden as needed, here is the default lifecycle:

1. `LoadModules`
This is where you should load your own modules, the base.LoadModules() will load the default framework so if you do not want this and want to load your own optimized framework components just dont call the base version. An example of this is shown in the optimized performance tests where we are manually assigning the component type ids so we do not want the default loader.

2. `LoadPlugins`
This is where you should load any plugins you want to use, if you have no plugins to use then dont bother overriding it.

3. `ResolveApplicationDependencies`
This is where the dependencies of the application are manually resolved from the DI Container, so the ISystemExecutor and IEventSystem etc are all resolved at this point, once all plugins and modules are run. The base.ResolveApplicationDependencies() will setup the core EcsRxApplication dependencies so you should call this then resolve anything specific you need after this point.

4. `BindSystems`
This is where all systems are BOUND (which means they are in the DI container but not resolved), by default it will auto bind all systems within application scope (using BindAllSystemsWithinApplicationScope), however you can override and remove the base call if you do not want this behaviour, or if you want to manually register other systems you can let it auto register systems within application scope and then manually bind any other systems you require.

5. `StartSystems`
This is where all systems that are bound should be started (they are added to the ISystemExecutor), by default this stage will add all bound systems to the active system executor (using StartAllBoundSystems), however you can override this behaviour to manually control what systems are to be started, but in most cases this default behaviour will satisfy what you want.

6. `ApplicationStarted`
At this point everything you need should be setup ready for you to start creating collections, entities and starting your game.
