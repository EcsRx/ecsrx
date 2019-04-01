# Stuff To Know

Before we begin there should be some existing knowledge in place, if you don't know the stuff below then just go read up on it (each heading a link you can click on to learn about it). You don't **NEED** to know all of this, but it will be **VERY** beneficial.

## [ECS Pattern](https://grofit.gitbook.io/development-for-winners/development/game-dev/patterns/ecs)

This covers what ECS is, and how components, entities and systems all interact within the eco system, rather than writing another *"What is ECS"* set of documents it's easier to just read the link above or google the subject if you don't know what ECS is.

## [Inversion Of Control](https://grofit.gitbook.io/development-for-winners/development/general/dependency-patterns/inversion-of-control)

This is about how your objects (generally classes) get their dependencies, this approach is used heavily throughout the codebase and means most of what the class needs is passed in via its constructor.

## [Dependency Injection](https://grofit.gitbook.io/development-for-winners/development/general/dependency-patterns/dependency-injection)

This is related to IoC and is basically a industry standard way of resolving dependencies for objects in a way that is flexible and highly configurable. If you have ever seen any code with bits like `Bind<ISomething>().To<Something>()` then that is DI configuration.

## [Reactive Extensions (RX)](https://grofit.gitbook.io/development-for-winners/development/general/data-patterns/reactive-extensions)

This library tries to make use of the common ECS paradigm while making reactivity one of its big bonuses. If you are unsure about what rx is or how it can benefit your project then have a read into it, simply speaking its a push approach to data changing rather than a polling approach.

## [Intro to Unit Testing](https://grofit.gitbook.io/development-for-winners/development/general/testing/intro-to-testing)

If you have not done any unit testing, then its not the end of the world, you don't **NEED** to suddenly become a testing rockstar but part of the benefit of using this framework is it tries to keep your code highly testable, so if you want to make use of that benefit its worth knowing how.

## [Mocking in Unit Tests](https://grofit.gitbook.io/development-for-winners/development/general/testing/mocking)

Related to unit testing, it covers how you can provide mocked/fake implementations of dependencies to force your code to act a certain way and then confirm that it behaves as you expect.

## WHAT IF I STILL NEED HELP OR HAVE QUESTIONS!??!

If you want to know more on the above topics then feel free to drop into our [Discord Channel](https://discord.gg/bS2rnGz) to discuss further or ask any questions.