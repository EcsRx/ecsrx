# System Affinities for collections

Out the box `ObservableGroups` will just listen to changes across all collections, but you can give them an affinity so they will only listen to changes on certain collections, providing a performance boost as they dont need to listen to changes on entities they will never interact with, however most of the time you are not creating observable groups as they are requested per system. So we need to be able to tell the system what affinity they have so you can have a better suited observable group.

```csharp
// Tell this system that it should only interact with collections with id 1,5,6
[CollectionAffinity(1,5,6)]
public class SomeSystemWithAffinity : ISetupSystem
{
    // ...
}
```

If no affinity is provided then the observable group requested will just listen for changes on all collections, however if an affinity is provided you will only be listening to changes on the collections you care about.

## How much of a performance boost is this?

**It's a big boost.**

One of the slowest parts of the system is group resolving, as every change has to be checked against observable groups to see if it needs to pump out changes to systems, i.e a component being added may invalidate an entity in a group, or cause an entity to be added to a group.

So although the checking is quite fast, it happens SOOOOOOOooooo often that it can take up a sizeable chunk of time the more groups and entities you have, as out the box each change will go to each observable group to ignore/process.

You wont really notice the difference until you have tens of observable groups and hundreds of entities, but if you have thousands of entities and hundreds of observable groups you will be thankful you have partitioned out to reduce chatter.