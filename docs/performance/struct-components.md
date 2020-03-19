# Struct Components

Out the box it assumes you will be using classes for components, i.e:

```csharp
public class MyComponent : IComponent
{
	//...
}
```

For most use cases this will be fine, however using classes means that the underlying data cannot be properly organized in memory. So if you are wanting to batch your systems you can only batch the component references with classes, not their underlying data, but with `struct` based components the data is stored with the struct so if you are looking to batch you have all the value type data there in a big line to just chug away at making it far more efficient.

## How do we use this then?

Its not a massive leap, just change your `class` to a `struct` and implement the same interface, sounds simple right!

There is however some other effort required on your part to use structs... So first of all you will end up needing to provide the index lookups to the `entity.GetComponent<T>(int lookupId)` method, this will now return a `ref` object that you can edit if you want and for all intents and purposes will act like a reference type.

There is also some other quirks, such as if you are using batched systems (advised that you only really use structs if you are planning to use batched systems with it), where you will be working with `ref` arguments to simplify your interactions.

## How much of a performance boost does it give?

**Quite a big boost**

It will generally reduce your memory consumption, it will speed up a lot of lookups and will also mean the batched systems have all the data ready to go in memory ahead of time, making it faster than the class based batched systems.

This all being said there is some mental overhead to managing structs due to them being value types so given that the speed difference between class based batched systems and struct based ones is anywhere from 10%-30% in most common use cases tested if you dont need that extra performance benefit you may not need to worry about the overhead of using structs.