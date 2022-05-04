# EcsRx

EcsRx is a reactive take on the common ECS pattern with a well separated design using rx and adhering to IoC and other sensible design patterns.

[![Build Status][build-status-image]][build-status-url]
[![Code Quality Status][codacy-image]][codacy-url]
[![License][license-image]][license-url]
[![Nuget Version][nuget-image]][nuget-url]
[![Join Discord Chat][discord-image]][discord-url]
[![Documentation][gitbook-image]][gitbook-url]

## Features

- Simple ECS interfaces to follow
- Fully reactive architecture
- Favours composition over inheritance
- Adheres to inversion of control
- Lightweight codebase 
- Built in support for events (raise your own and react to them)
- Built in support for pooling (easy to add your own implementation or wrap 3rd party pooling tools)
- Built in support for plugins (wrap up your own components/systems/events and share them with others)

The core framework is meant to be used primarily by .net applications/games, there is a unity specific version [here](https://github.com/ecsrx/ecsrx.unity) which builds on top of this core version, as well as a monogame game version [here](https://github.com/ecsrx/ecsrx.monogame).

> It is also worth mentioning that this framework builds on top of [SystemsRx](https://github.com/EcsRx/SystemsRx), which can be used without EcsRx for when you want Systems and Events but dont need the Entity Component aspects.

## Installation

The library was built to support .net standard 2.0, so you can just reference the assembly, and include a compatible rx implementation.

## Quick Start

It is advised to look at the [setup docs](./docs/introduction/setup.md), this covers the 2 avenues to setup the application using it without the helper libraries, or with the helper libraries which offer you dependency injection and other benefits.

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

Check the examples for more use cases, and the unity flavour of ecsrx which has more examples and demo projects, and drop into the discord channel to ask any questions.

## Running Examples

If you want to run the examples then just clone it and open examples project in the `src` folder, then run the examples, I will try to add to as the library matures.

There are also a suite of tests which are being expanded as the project grows, it was written with testability in mind.

## Architecture / Infrastructure

### EcsRx

This is layered on top of **SystemsRx** and adds the ECS paradigm to the framework as well as adding a couple of systems specifically for entity handling. This also contains an **EcsRx.Infrastructure** layer which builds off the **SystemsRx.Infrastructure** layer to provide additional ECS related infrastructure.

### EcsRx.Infrastructure

This builds on top of the **SystemsRx.Infrastructure** library to provide all the cool DI abstraction and plugin stuffs.

### EcsRx.Plugins.*

These are plugins that can be used with **EcsRx** to add new functionality to the library, from entity persistance/rehydration to schedulable systems, these can optionally be added to your projects.

### History
The library was originally developed for unity (way before they had their own ECS framework) and since then has moved to be a regular .net library that can run anywhere (Even in Blazor WASM).

Off the back of this there have been a few other libraries that were created for EcsRx but now live as their own standalone libraries:

- [LazyData](https://github.com/grofit/LazyData) - Was created as a serialization layer for EcsRx
- [Persistity](https://github.com/grofit/Persistity) - Was created as an ETL style pipeline for EcsRx
- [SystemsRx](https://github.com/ecsrx/systemsrx) - Was the supporting system/data layer for EcsRx

## Docs

There is a book available which covers the main parts which can be found here:

[![Documentation][gitbook-image]][gitbook-url]

> This is basically just the [docs folder](docs) in a fancy viewer

## Community Plugins/Extensions

This can all be found within the [docs here](./docs/others/third-party-content.md)


[build-status-image]: https://ci.appveyor.com/api/projects/status/55d1256yrra6fmls/branch/master?svg=true
[build-status-url]: https://ci.appveyor.com/project/grofit/ecsrx/branch/master
[nuget-image]: https://img.shields.io/nuget/v/ecsrx.svg
[nuget-url]: https://www.nuget.org/packages/EcsRx/
[discord-image]: https://img.shields.io/discord/488609938399297536.svg
[discord-url]: https://discord.gg/bS2rnGz
[codacy-image]: https://app.codacy.com/project/badge/Grade/6ea8dc8c37f3452fa3a5a8dc6b0b3f47
[codacy-url]: https://www.codacy.com/gh/EcsRx/ecsrx/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=EcsRx/ecsrx&amp;utm_campaign=Badge_Grade
[license-image]: https://img.shields.io/github/license/ecsrx/ecsrx.svg
[license-url]: https://github.com/EcsRx/ecsrx/blob/master/LICENSE
[gitbook-image]: https://img.shields.io/static/v1.svg?label=Documentation&message=Read%20Now&color=Green&style=flat
[gitbook-url]: https://ecsrx.gitbook.io/project/
