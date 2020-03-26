# Dependency Injection Abstraction

As this framework is built to enable plugin development and work on any platform/framework, there has been some efforts to streamline how DI is handled within the framework by creating an abstraction over the underlying DI system.

## But why?

So lets say you wanted to create an RPG plugin where it contained components, events, systems etc for handling buffs, items, inventory etc. Now none of this logic really is dependent on Unity, Monogame etc... its all just raw .net, but you will want to setup the DI concerns for this plugin so it can be loaded into any of those platforms and just work.

To be able to do that we need to have the notion of DI in the framework without having an ACTUAL DI container available. As there would be no point having a hard dependency on Zenject for your plugin if you wanted to consume it in the Monogame world etc.

## Functionality

As not all DI frameworks provide the same syntax/feature set we have streamlined the approach to include most common scenarios, and will add to it if needed. Here is a list of the container and config supported features.

### Container Features

#### Binding - `Bind<From, To>`, `Bind<T>`, `Unbind<T>`

This lets you bind a given type to another type, so it could be `Bind<ISomething, Something>()` or if you want to self bind the concrete type just do `Bind<Something>()`. You can also unbind a type by using `Unbind<Something>()`. You can also add named bindings and other configuration by passing in a configuration object (discussed further on).

#### Resolving -  `Resolve<T>`, `ResolveAll<T>`

This lets you get an instance of something from the DI container, if you want a single instance do `Resolve<T>()` or if you want all instances matching that type do `ResolveAll<T>()` which returns an enumerable of the given type. You can also request an instance of a type with a given name providing you have bound the type with a name, by doing `Resolve<ISomething>("something-1")` which would return the implementation of `Something` named "something-1".

#### Modules/Setup - `LoadModule<T>`, `LoadModule(IDependencyModule)`

There is also support for creating modules that setup your DI configuration, this requires you to implement `IDependencyModule` and has a `Setup` method which provides you the container to setup your bindings on.

### Binding Configuration

If you want to configure how a binding should work, you can pass into the `Bind` methods an optional configuration object which exposes the current properties.

### AsSingleton: `bool`

Setting this to `true` will mean that only one instance of this binding should exist, so if you were to do:
```csharp
Bind<IEventSystem, EventSystem>(new BindingConfiguration{AsSingleton = true});
```
Then resolve `IEventSystem` in multiple places you will always get back the same instance, which is extremely handy for infrastructure style objects which should act as singletons. If you provide `false` as the value it will return a new instance for every resolve request.

### WithName: `string`

This will allow you to give the binding a name for resolving via name.

### OnActivation: `Action<IDependencyContainer, object>`

This method will fire when an instance has been activated within the DI container.

### WhenInjectedInto: `Type || IEnumerable<Type>`

This will only inject the given binding in when the requesting type matches the types provided.

### ToInstance: `object`

This allows you to bind to an actual instance of an object rather than a type, which is useful if you need to manually setup something yourself.

```csharp
var someInstance = new Something(foo, bar);
Bind<ISomething>(new BindingConfiguration{ToInstance = someInstance});
```

### ToMethod: `Func<IDependencyContainer, object>`

This allows you to lazy bind something to a method rather than an instance/type, which is useful if you want to setup something in a custom way once all DI configuration has been processed.

```csharp
var bindingConfiguration = new BindingConfiguration({
    ToMethod: container =>
      {
          var foo = container.Resolve<Foo>();
          return new Something(foo, "woop woop");
      });
});
Bind<ISomething>(bindingConfiguration);
```

There is a slightly nicer way to set this up using a builder pattern extension shown further on.

### WithNamedConstructorArgs: `IDictionary<string, object>`

This allows you to provide an argument for the constructor of the type based off its argument name.


### WithTypedConstructorArgs: `IDictionary<Type, object>`

This allows you to provide an argument for the constructor of the type based off its argument type.

## Extension helpers

The configuration object is simple but can be unsightly for larger configurations, to assist in this we have added a couple of extension methods which can be used to make your binding config a little more succinct.

### Builder helpers

There is a builder pattern helper which lets you setup your binding config via a builder rather than an instance of `BindingConfiguration`, this can be used by just creating a lambda within the bind method like so:

```csharp
Bind<ISomething>(config => config
  .AsSingleton()
  .WithName("something-1")
  .WithConstructorArg("foo", 10)
});
```

This lets you setup the configuration in a nice way, it also has type safety so you can setup instances and methods using it like so:

```csharp
// With instance
Bind<ISomething>(config => config
  .ToInstance(new InstanceOfISomething())
});

// Wouldnt work, incorrect type
Bind<ISomething>(config => config
  .ToInstance(new InstanceOfISomethingElse()) // error, not ISomething
});

// With method
Bind<ISomething>(config => config
  .ToMethod(container =>
  {
      var foo = container.Resolve<Foo>();
      return new Something(foo, "woop woop");
  })
});
```

### Other helpers

There is also another helper which allows you to get an `IObservableGroup` directly from the container, this internally gets the instance of the `IEntityCollectionManager` and requests an observable group of a given type like so:

```csharp
// By group
var observableGroup = container.ResolveObservableGroup(new MyGroup());
// By required components
var observableGroup = container.ResolveObservableGroup(typeof(PlayerComponent));
```

This can be handy when you want to setup `Computed` objects which computed from a group, an example of this can be seen in the [Roguelike example](https://github.com/EcsRx/ecsrx.roguelike2d/blob/master/Assets/Game/Modules/ComputedModule.cs).