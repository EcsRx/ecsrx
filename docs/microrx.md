# MicroRx .... wtf is that?

## tldr;

Both rx.net and unirx have problems on different engines/platforms so EcsRx core cannot depend on either, so it depends on this TINY rx implementation so it can work with either.

## Why?

So in .net currently there are 2 main rx implementations:
 
 - `System.Reactive` (aka `dotnet reactive` or `rx.net`)
 - `UniRx` (a unity specific rx implementation)
 
 Herein lies the problem, so this library was first created as a unity project, then it was split off into a generic .net framework agnostic of engines and frameworks, with a unity layer (and other engine specific layers) which would sit on top of this.
 
 ## The ACTUAL problem
 
 Unity and a lot of other engines/frameworks (Monogame, Godot, Xenko etc) support modern .net syntax and libraries, and they allow you to release to different platforms, and this is where the problem rears its head.
 
 As certain platforms don't support JIT and require AOT compilation, which the normal `System.Reactive` framework doesn't support. There were however some discussions on the work needed to allow it to work in AOT scenarios, but no solid work seems to have resulted from that yet.
 
 This then means that if you want to target iOS for example you cannot use `System.Reactive` out the box, so for unity its not really possible to use it due to IL2CPP and certain platform constraints.
 
 ## UNIRX THEN? SURELY THAT WORKS?
 
 If you are in Unity then yes, UniRx will work in all of those scenarios (with some tweaks). HOWEVER, UniRx is not really maintained that well these days as the maintainer drops in every now and again and then vanishes and no one else has been able to assist in maintaining the project.
 
 This also means that if you are not using Unity you cannot really use UniRx. While there is a nuget package for it, it has not been updated in years and given the lack of communication on the main releases of unirx to the asset store and github I would not hold my breath for a new version of that any time soon (Believe me, I wont stop pestering the maintainer of it).
  
 ## So you made your own Rx?
 
 Kinda, so EcsRx originally made use of fancy rx linq stuff and some other fancy rx bits, but I couldn't have a dependency on `rx.net` as then the unity layer would fail, and I couldnt have a dependency on `UniRx` as its nuget package is wildly outdated and the other part cannot be used outside of unity.
 
 So because of these problems I decided to make a TINY rx implementation, which is pretty much a rip off and simplification of parts of `UniRx`. This allows EcsRx to internally be able to make use of basic rx paradigms like `Subject` and `CompositeDisposable` (and a few other bits) but has very little else, i.e no Linq stuff, no scheduler, no async stuffs.
 
 This makes it a very small library and very specific in its usecase, so as EcsRx uses `IObservable` as its contract which is a part of .net it means the stuff inside `MicroRx` should be compatible with unity and/or any other framework/engine, and place nicely with other more feature rich rx implementations, so you get to decide if you want to use `rx.net` or `unirx` (or any other rx implementation) in your consuming project, while the EcsRx core is blissfully unaware of what you are using.
 
 ## Going forward
 
 I don't really want to have a micro rx implementation, I would much rather depend on a better, more maintained version of rx, but until `rx.net` works in unity on all platforms, or `UniRx` starts releasing maintained nuget packages which can be consumed on any platform I am going to need to rely upon this to polyfill those needs and stay agnostic of implementation.
 
 Really you dont need to worry about this and dont even really need to care about `MicroRx`'s existence, although I would say DONT USE IT FOR ANYTHING MAJOR, its only really there to polyfill the internal EcsRx libs.