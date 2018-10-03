# Setup

Right now setup is not quite as simple as we would like, however this is mainly due to some shifting sands in the rx area as well as still mapping the unity infrastructure fully over to the core.

## To DI or not DI

So if you dont know what DI (Dependency Injection) is I recommend you go [read this](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/dependency-injection.html) and [this](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/inversion-of-control.html) which will give you a quick overview on what IoC (Inversion of Control) and DI is and how you use it.

Now you hopefully know what DI is you can now be in a position to decide if you really care about that and design patterns, if not then you can just take the `EcsRx` core library and knock up your own bare bones bootstrapper, if however DI and design patterns are important to you then you can use (and are advised to) use the `EcsRx.Infrastructure` module too, which contains a lot of common setup and conventions.

### I dont care for your design patterns sir, just let me get going

Ok captain, this is like the most bare bones setup I would advise:

```csharp
public abstract class EcsRxApplication
{
	public ISystemExecutor SystemExecutor { get; }
	public IEventSystem EventSystem { get; }
	public IEntityCollectionManager EntityCollectionManager { get; }

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
		var observableGroupFactory = new DefaultObservableObservableGroupFactory();
		EntityCollectionManager = new EntityCollectionManager(entityCollectionFactory, observableGroupFactory, componentLookup);

		// All system handlers for the system types you want to support
		var reactsToEntityHandler = new ReactToEntitySystemHandler(EntityCollectionManager);
		var reactsToGroupHandler = new ReactToGroupSystemHandler(EntityCollectionManager);
		var reactsToDataHandler = new ReactToDataSystemHandler(EntityCollectionManager);
		var manualSystemHandler = new ManualSystemHandler(EntityCollectionManager);
		var setupHandler = new SetupSystemHandler(EntityCollectionManager);
		var teardownHandler = new TeardownSystemHandler(EntityCollectionManager);

		var conventionalSystems = new List<IConventionalSystemHandler>
		{
			setupHandler,
			teardownHandler,
			reactsToEntityHandler,
			reactsToGroupHandler,
			reactsToDataHandler,
			manualSystemHandler
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

### I want all the design patterns please

A wise choice sir, so in this instance things are currently not quite as nice as we would like outside the unity use case.

So to start with its advised that you take:

- `EcsRx`
- `EcsRx.Systems`
- `EcsRx.Infrastructure`
- `EcsRx.Views`

This will provide the basic classes for you to extend, however one fundemental problem is that the infrastructure expects you to be using a DI framework. It doesn't really care which DI framework as it provides an interface for you to implement and then consume that in your own `EcsRxApplication` implementation. Like mentioned above this is all due to change soon as once things are finalized and the unity infrastructure and view stuff has been decoupled it will be easier to provide more "out the box" implementations, but for the moment you will need to:

- Implement `IDependencyContainer` for your DI framework of choice. [Here is a Ninject one from examples](https://github.com/EcsRx/ecsrx/blob/master/src/EcsRx.Examples/Dependencies/NinjectDependencyContainer.cs)
- Implement your own `EcsRxApplication` class, giving it an `IDependencyContainer` implementation to use. [Here is one from examples](https://github.com/EcsRx/ecsrx/blob/master/src/EcsRx.Examples/Application/EcsRxConsoleApplication.cs)
- Extend your custom `EcsRxApplication` implementation for each logical app you need to make

Once you have done those the first 2 things you are ready to roll and the 3rd step is basically using those infrastructure pieces yourself to get going.

It is worth noting here that this is EXACTLY how the examples work in this project so its worth cracking them open to see how its all done, but the same principals can be applied to your own applications.

## Going forward

So ideally once things settle and there is a working core with unity (the biggest EcsRx consumer) then we can look at adding pre made `EcsRx.Infrastructure.Ninject` etc which will contain pre-made "drop in" application classes for you to use rather than rolling your own, but we are not at that point yet.
