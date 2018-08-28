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

Chances are it is not, its just different. Like Entitas is great and is very similar to this project, however it doesn't really have a complex reaction system, so anything you want to do reactively is a bit of a pain. Other than that this is pretty much the same sort of thing as Entitas. Compared to uFrame ECS it doesnt have the entity == gameobject/etc convention mentioned elsewhere but it has all the reactive stuff and it also works nicely with the notion of models, which uFrame ECS requires type references for which loses a good part of the benefit of using the framework.

So ultimately this is not marketed as the one ECS system to rule them all, its just got what I would deem the nice bits of a few existing ones.

It is also pretty succinct as all you need to do is implement a couple of interfaces, the framework setup is trivial (and done for you if you use the unity layer package) and it also can work nicely with DI allowing you to have more config driven aspects to your game without passing random garbage around or having horrible statics/singletons to do this.