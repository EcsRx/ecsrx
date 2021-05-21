# Group Binding Plugin

Please note that this plugin isn't enabled by default and you have to load it yourself.

Sometimes you just want an easy access to an `IObservableGroup` based on a group without having to start injecting a `IObservableGroupManager` by hand every time to provide you with it.
Using this Plugin enables you to just add a `FromGroupAttribute` or a `FromComponentsAttribute` to any public field or any property with a public setter of the type `IObservableGroup` on your system.

If your system is an `IGroupSystem` you can also use the `FromGroupAttribute` without even supplying a group to it and it'll fall back to using the value from the `IGroupSystem.Group`.

# System Affinities in this plugin

Please refer to the article about system affinities within the performance category for general info about the concept.

By accompanying your `FromGroupAttribute`/ `FromComponentsAttribute` by a `CollectionAffinityAttribute` you can specify which collectionIds are to be observed the specified group.
If you are using the `FromGroupAttribute` without supplying any arguments, it'll also consider a `CollectionAffinityAttribute` on your systems class. However you can still overwrite this by also supplying a `CollectionAffinityAttribute` to that member.
