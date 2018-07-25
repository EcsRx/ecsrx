# EcsRx

EcsRx is a reactive take on the common ECS pattern with a well separated design using rx and adhering to IoC and other sensible design patterns.

[![Build Status][build-status-image]][build-status-url]
[![Nuget Version][nuget-image]][nuget-url]
[![Join Gitter Chat][gitter-image]][gitter-url]

## Features

- Simple ECS interfaces to follow
- Fully reactive architecture
- Favours composition over inheritance
- Adheres to inversion of control
- Lightweight codebase 
- Built in support for events (raise your own and react to them)
- Built in support for pooling (easy to add your own implementation or wrap 3rd party pooling tools)
- Built in support for plugins (wrap up your own components/systems/events and share them with others)

The core framework is meant to be used primarily by .net applications/games, there is a unity specific version [here](https://github.com/ecsrx/ecsrx.unity) which is currently being ported over to use this version.

## Installation

The library was built to support .net standard 2.0, so you can just reference the assembly, and include a compatible rx implementation.

## Quick Start

It is advised to look at the examples, which show the [bare bones required setup](src/EcsRx.Examples/HelloWorldExample/HelloWorldExample.cs), this is just an example which uses a console based application, in the real world you will probably be targetting Unity or Monogame or maybe even Godot etc, but all that differs is your `EcsRxApplication` implementation (assuming you are using the pre-made infrastructure module).

If you are using unity it is recommended you just ignore everything here and use the instructions on the [ecsrx.unity repository](ttps://github.com/ecsrx/ecsrx.unity) as that has not been fully mapped over to use this core version yet so is its own eco system until that jump is made.

### Simple components

```csharp
public class HealthComponent : IComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}
```

You implement the `IComponent` interface which marks the class as a component, and you can optionally implement `IDisposable` if you want to dispose stuff like so:

```csharp
public class HealthComponent : IComponent, IDisposable
{
    public ReactiveProperty<int> CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    
    public HealthComponent() 
    { CurrentHealth = new ReactiveProperty<int>(); }
    
    public void Dispose() 
    { CurrentHealth.Dispose; }
}
```

Any component which is marked with `IDisposable` will be auto disposed of by entities.

### Simple systems

```csharp
public class CheckForDeathSystem : IReactToEntitySystem
{
    public IGroup TargetGroup => new Group(typeof(HealthComponent)); // Get any entities with health component

    public IObservable<IEntity> ReactToEntity(IEntity entity) // Explain when you want to execute
    {
        var healthComponent = entity.GetComponent<HealthComponent>();
        return healthComponent.CurrentHealth.Where(x => x <= 0).Select(x => entity);
    }
    
    public void Process(IEntity entity) // Logic run whenever the above reaction occurs
    {
        entity.RemoveComponent<HealthComponent>();
        entity.AddComponent<IsDeadComponent>();
    }
}
``` 

Systems are conventional, so there are many built in types like `IReactToEntitySystem`, `IReactToGroupSystem`, `IManualSystem` and many others, but you can read about them in the [docs/systems](docs/systems.md), you can add your own conventional systems by extending `ISystem` and systems are handled for you by the `ISystemExecutor`.

Check the examples for more use cases, and the unity flavour of ecsrx which has more examples and demo projects, and drop into the gitter channel to ask any questions.

## Running Examples

If you want to run the examples then just clone it and open examples project in the `src` folder, then run the examples, I will try to add to as the library matures.

There are also a suite of tests which are being expanded as the project grows, it was written with testability in mind.

## Note on infrastructure/view namespaces and going forward

This library started out as a unity specific project but has moved to a generic .net library, due to this the movement of functionality from the unity layer down into the core has been incremental. 

We are now at a point where the underlying infrastructure module (mainly for `EcsRxApplication` and dependency injection notions) has been added, and the generic view module has been moved here. While both of these libraries offer a stepping stone to get up and running quicker they unfortunately are not as easy as just including and off you go.

The examples folder shows examples on how to create your own application implementations, but hopefully once things have been ironed out in the whole Rx world this area will improve more as we start to add another layer which will let you just drop in and go (like the unity version).

If you want to know more about this drop into the gitter chat and we can discuss more.

## Docs

See the [docs folder](docs) for more information. (This will grow)

[build-status-image]: https://ci.appveyor.com/api/projects/status/55d1256yrra6fmls/branch/master?svg=true
[build-status-url]: https://ci.appveyor.com/project/grofit/ecsrx/branch/master
[gitter-image]: https://badges.gitter.im/grofit/ecsrx.svg
[gitter-url]: https://gitter.im/grofit/ecsrx
[nuget-image]: https://img.shields.io/nuget/v/ecsrx.svg
[nuget-url]: https://www.nuget.org/packages/EcsRx/
