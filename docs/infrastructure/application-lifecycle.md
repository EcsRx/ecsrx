# Application Lifecycle

The infrastructure aspect of the library provides a default application class that should be inherited from and overidden as needed. This provides a way to keep applications consistent and to make sure that things load when you expect. Here is the default lifecycle methods and when they will run in the process of an application starting.

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