# HELLO

Welcome etc!

There are a few things I recommend you know before you start using this framework. You don't **HAVE** to know them, but it will help.

So if you know all the below points feel free to continue on to the rest of the docs, if not then I suggest you have a quick look at the links and google the subject and try to get an understanding of it

- [ECS Pattern](https://grofit.gitbooks.io/development-for-winners/content/development/logic-patterns/ecs.html)
- [Inversion Of Control](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/inversion-of-control.html)
- [Dependency Injection](https://grofit.gitbooks.io/development-for-winners/content/development/dependency-patterns/dependency-injection.html)
- [Reactive Extensions (RX)](https://grofit.gitbooks.io/development-for-winners/content/development/data-patterns/reactive-extensions.html)
- [Intro to Unit Testing](https://grofit.gitbooks.io/development-for-winners/content/development/testing/intro-to-testing.html)

If you want to know more on the above topics then feel free to drop into our [Gitter Channel](https://gitter.im/grofit/ecsrx) to discuss further or ask questions.

So with that stuff out of the way I would recommend that you look into the common ECS docs in here (components, entities, groups), then look at the more advanced features offered here.

- [Observable Groups](./observable-groups.md)

These are a really powerful concept and provide you a way to view what entities are in a store and be notified when they are added/removed.

- [Conventional Systems](./systems.md)

Normally ECS provides a singular type of system but as we have the notion of reactive groups we can notify systems more intelligently and use systems with conventions to save time and boilerplate code.

- [Computeds](./computeds.md)

Computeds are like a living object allow you to pre-compute a value/collection/group based off some logic and the value will continually be updated as things change.