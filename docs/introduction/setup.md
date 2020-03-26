# Setup

If you are using Monogame or Unity you can just go and get the platform specific helpers which will out the box provide you most of the infrastructure and setup required to get going, so go look there.

---

If you are not using Unity or Monogame and want to pioneer into a new territory then its just a case of setting up the bits that are needed for the core parts of the system to run.

## Pre build infrastructure or not?

Out the box EcsRx comes with a load of infrastructure classes which simplify setup, if you are happy to use that as a basis for setup then your job becomes a bit simpler.

There is also a whole section in these docs around the infrastructure and how to use the application and other classes within there in common scenarios.

### YES I WANT ALL THE INFRASTRUCTURE PLX PLX

A wise choice so to start with its advised that you take:

- `EcsRx`
- `EcsRx.Infrastructure`
- `EcsRx.Plugins.ReactiveSystems`
- `EcsRx.Plugins.Views`

This will provide the basic classes for you to extend, however one fundamental problem is that the infrastructure expects you to be using a DI framework. It doesn't really care which DI framework as it provides an interface for you to implement and then consume that in your own `EcsRxApplication` implementation.

So here are the main bits you need:

- Implement `IDependencyContainer` for your DI framework of choice. [Here is a Ninject one from examples](https://github.com/EcsRx/ecsrx/blob/master/src/EcsRx.Examples/Dependencies/NinjectDependencyContainer.cs)

- Implement your own `EcsRxApplication` class, giving it an `IDependencyContainer` implementation to use. [Here is one from examples](https://github.com/EcsRx/ecsrx/blob/master/src/EcsRx.Examples/Application/EcsRxConsoleApplication.cs)

- Extend your custom `EcsRxApplication` implementation for each logical app you need to make, as shown in the EcsRx console examples

There are pre-made DI implementations for **Ninject** and **Zenject** so if you can use one of those on your platform GREAT! if not then just pick a DI framework of choice and implement your own handler for it (using the ninject one as an example to base it off).

> So if you dont know what DI (Dependency Injection) is I recommend you go [read this](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/dependency-injection.html) and [this](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/inversion-of-control.html) which will give you a quick overview on what IoC (Inversion of Control) and DI is and how you use it.

It is worth noting here that this is EXACTLY how the examples work in this project so its worth cracking them open to see how its all done, but the same principals can be applied to your own applications.

### NOOOPE! I dont care for your design patterns sir, just let me get going

Ok captain, if you just want to get things going with minimum effort then I would just get the core lib and manually instantiate everything that is needed. 

This is like the most bare bones setup I would advise:

```csharp
public abstract class EcsRxApplication
{
	public ISystemExecutor SystemExecutor { get; }
	public IEventSystem EventSystem { get; }
	public IObservableGroupManager ObservableGroupManager { get; }
	public IEntityDatabase EntityDatabase { get; }

	protected EcsRxApplication()
	{
		// For sending events around
		EventSystem = new EventSystem(new MessageBroker());
		
		// For mapping component types to underlying indexes
		var componentTypeAssigner = new DefaultComponentTypeAssigner();
		var allComponents = componentTypeAssigner.GenerateComponentLookups();
		
		var componentLookup = new ComponentTypeLookup(allComponents);
		// For interacting with the component databases
		var componentDatabase = new ComponentDatabase(componentLookup);
		var componentRepository = new ComponentRepository(componentLookup, componentDatabase);	
		
		// For creating entities, collections, observable groups and managing Ids
		var entityFactory = new DefaultEntityFactory(new IdPool(), componentRepository);
		var entityCollectionFactory = new DefaultEntityCollectionFactory(entityFactory);
		EntityDatabase = new EntityDatabase(entityFactory);
		var observableGroupFactory = new DefaultObservableObservableGroupFactory();
		ObservableGroupManager = new EntityCollectionManager(observableGroupFactory, entityDatabase, componentLookup);

		// All system handlers for the system types you want to support
		var manualSystemHandler = new ManualSystemHandler(ObservableGroupManager);
		var basicSystemHandler = new BasicSystemHandler(ObservableGroupManager);

		var conventionalSystems = new List<IConventionalSystemHandler>
		{
			manualSystemHandler,
            basicSystemHandler
		};
		
		// The main executor which manages how systems are given information
		SystemExecutor = new SystemExecutor(conventionalSystems);
	}

	public abstract void StartApplication();
}
```

Then all you need to do is go:

```csharp
public class HelloWorldExampleApplication : EcsRxApplication
{
	public override void StartApplication()
	{
		SystemExecutor.AddSystem(new TalkingSystem());

		var defaultCollection = EntityCollectionManager.GetCollection();
		var entity = defaultCollection.CreateEntity();

		var canTalkComponent = new CanTalkComponent {Message = "Hello world"};
		entity.AddComponent(canTalkComponent);
	}
}
```

HUZZAH! you are now up and running and you can make your own conventions or design patterns around this.