# Plugins

There are a few plugins provided with EcsRx that you can opt in to use by just referencing the dll and loading the plugin from your application class in the `LoadPlugins` phase. Anyone in the community can make their own plugins and hopefully going forward there will be more plugins available.

## How to use plugins?

You just need to reference the dll in your project and then in your `LoadPlugins` phase you just load the desired plugin entry point. Here is an example of loading some of the official EcsRx plugins:

```csharp
public abstract class EcsRxConsoleApplication : EcsRxApplication
{
    public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();

    protected override void LoadPlugins()
    {
        // Register the plugins we want to use
        RegisterPlugin(new ReactiveSystemsPlugin());
        RegisterPlugin(new ComputedsPlugin());
        RegisterPlugin(new ViewsPlugin());
        RegisterPlugin(new BatchPlugin());
    }

    protected override void StartSystems()
    {
        this.StartAllBoundSystems();
    }
}
```

Once they are registered there the application will setup any dependencies for the plugin and kick start anything that needs to be running.

## How to make plugins

Plugins are pretty simple, they just require you to implement the `IEcsRxPlugin` interface and that's it.

Here is an example of the reactive systems plugin, which binds some conventional system handlers for the `SystemExecutor` to make use of, then we output any systems we need to register (in this case none).

```csharp
public class ReactiveSystemsPlugin : IEcsRxPlugin
{
    public string Name => "Reactive Systems";
    public Version Version { get; } = new Version("1.0.0");
    
    public void SetupDependencies(IDependencyContainer container)
    {
        container.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
        container.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
        container.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
        container.Bind<IConventionalSystemHandler, SetupSystemHandler>();
        container.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
    }

    public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
}
```

There are plenty of example plugins in the core repo and there is also a buff one available on github too which was designed for unity originally but same concept applies.

## Some potential plugin use cases

- Sharing your own custom conventions (much like reactive systems and batched systems plugins)
- Sharing pre-made game logic, i.e a buffs, stealth, inventory etc
- Optimized implementations for specific platforms

You can wrap up almost anything into a shareable plugin, and if you are doing architectural plugins you can replace almost any part of EcsRx from within a plugin, or if you are making game logic you can use events/components as your contracts and just register your systems on setup so everything "just works" out the box on any platform (assuming you have no platform specific code in there).