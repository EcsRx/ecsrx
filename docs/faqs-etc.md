# FAQ/Blurbs

This project was inspired by [Entitas](https://github.com/sschmid/Entitas-CSharp) and [uFrame ECS](https://github.com/micahosborne/uFrame) and was an attempt to have some of the simplicity and separation of Entitas while having some of the nicer reactive elements of uFrame ECS, so a huge thanks to the creators of those 2 libraries.

It started off as a unity specific framework but has been separated to be used in both .net with rx, and in unity with unirx.

Now some common things you will probably want to ask/know:

## Generic Queries

### How Performant is it?

Seems ok, it is built to be functional and that is the first and foremost focus of the library, to make your life easier and put some pattern in place for interaction with things. 

Although performance is not the primary focus efforts have been made to make the framework peformant without sacrificing functionality or ease of use.

As everything adheres to IoC and is easily changed with custom implementations you can performance tweak the code how you see fit for your specific scenarios.

If you want to make some performance tests I would love to see them, we have some as part of the library to check on critical parts but it takes a lot of time to generate worthwhile scenarios, so feel free to write your own or provide us scenarios you want to see profiled.

### Why should I use it?

No reason really, if you like the ECS pattern but want to have reactive systems then this may be a good fit, it also can potentially be used in any .net application/framework so you could write your main game logic without even touching an engine.

### Is ECS better than MVVM/MVC/MVP/MGS?

Well Metal Gear Solid is a great game, so ECS is not better than that. It is also not *better* than the other patterns, it is just different. So MVVM is great for the enterprise developer coming from web/app development to game development or people who make UI heavy/Data centric games. MVP is very much the same but with the more formal separation on the data binding, and MVC for those who want to have more of a payload sort of approach for managing logic.

However you look at it, it's whatever you are used to. Believe it or not before I wrote this framework I had hardly any real world experience with ECS... I just liked some other ECS frameworks but always felt a bit was missing, be it view separation or reactivity etc... So this framework was created... maybe I should not be using ECS frameworks, let alone writing them...

### How is this any better than uFrame ECS, Entitas, Some other ECS system?

Chances are it is not, its just different. Like Entitas is great and is very similar to this project, however it doesn't really have a complex reaction system, so anything you want to do reactively is a bit of a pain. Other than that this is pretty much the same sort of thing as Entitas. Compared to uFrame ECS it doesnt have the entity == gameobject convention mentioned elsewhere but it has all the reactive stuff and it also works nicely with the notion of models, which uFrame ECS requires type references for which loses a good part of the benefit of using the framework.

So ultimately this is not marketed as the one ECS system to rule them all, its just got what I would deem the nice bits of a few existing ones.

It is also pretty succinct as all you need to do is implement a couple of interfaces, the framework setup is trivial (and done for you if you use the unity layer package) and it also can work nicely with DI allowing you to have more config driven aspects to your game without passing random garbage around or having horrible statics/singletons to do this

## Unity Specific Queries

### In Unity how can I use it with playmaker/visual coding tool etc

This is more geared for coders, you can easily communicate from playmaker or any other tool into the underlying *Systems* etc you would just need to write your own custom actions to bridge between this framework and the one you are consuming. If you are one of the people wanting this functionality then I am happy to assist you as I have written a lot of other libraries bridging frameworks in the unity world, especially Node Canvas.

### Why cant entities be GameObjects and components be MonoBehaviours?

If you like that style go check out uFrame ECS as that is built around that concept.

However assuming you want more of an explanation as to why, then there are many reasons, such as if you have a dependency on unity's objects it is impossible to do any mockable tests (unit/integration tests), so you can only test inside active scenes which are slow and cumbersome. They are also quite slow to work with, so to instantiate a new Entity is trivial, however to instantiate a new GameObject is not so trivial, then also MonoBehaviours have some overhead, whereas a component within this system is just a POCO so it generally makes things slicker and quicker while having a more decoupled framework.

There is also one other point worth making here and that is why do you need an in-scene representation of every entity? some entities may only live as data without a physical embodiment, or some entities may only need a physical object when they are within a certain distance of the player. Take for example an RTS game or RPG game where you have lots of NPC/Units moving around the game which you dont really care about rendering or doing anything in the scene with, these entities can easily live on in memory with little overhead still reacting to things without much of an overhead, but the moment you force everything to be a game object you automatically need to worry about this and disabling components when it is too far away etc.

#### Half way house on this approach

Now lets say you know all the above, and you want to make sure every entity exists in the scene with a game object, well its quite simple to achieve.

Make a component like so (unity scenario):

```
public class ViewComponent : IComponent
{
    public string PrefabType { get; set; } // optional
    public GameObject GameObject { get; set; }
}
```

Make sure every entity has this applied, then make a system to automatically create the object/prefab when this component exists, then make an extension method like so:

```
public static IEntityExtensions
{
    public static GetGameObject(this IEnity entity)
    {
        return entity.GetComponet<ViewComponent>().GameObject;
    }
}
```

This way if you make every entity have this component you can then easily just have `entity.GetGameObject()` to get the unity world stuffs, making it slightly easier to work with the unity world..

There is even a views extension which is provided which has a barebones implementation for views so it can be built upon for each engine, such as Unity, Godot, Monogame etc.