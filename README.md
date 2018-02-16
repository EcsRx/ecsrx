# EcsRx

EcsRx is a reactive take on the common ECS pattern with a well separated design using rx and adhering to IoC and other sensible design patterns.

[![Build Status][build-status-image]][build-status-url]
[![Join Gitter Chat][gitter-image]][gitter-url]

## New Info!

This is the future core library for ecsrx, the previous unity version will become a unity flavour of the ecsrx framework.

We have split this out and the unity version is available @ [grofit/ecsrx.unity](https://github.com/grofit/ecsrx.unity)

## Features

- Simple ECS interfaces to follow
- Fully reactive architecture
- Favours composition over inheritance
- Adheres to inversion of control
- Lightweight codebase 
- Built in support for events (raise your own and react to them)
- Built in support for pooling (easy to add your own implementation or wrap 3rd party pooling tools)
- Built in support for plugins (wrap up your own components/systems/events and share them with others)

The core framework is meant to be used primarily by .net applications/games, there is a unity specific version [here](https://github.com/grofit/ecsrx.unity)

## Installation

The library was built to support .net standard 2.0, so you can just reference the assembly, and include a compatible rx implementation.

## Quick Start

It is advised to look at the examples, which show the [bare bones required setup](src/EcsRx.Examples/Application/EcsRxApplication.cs), this is jus an example and we will look at having more support for specific frameworks going forward.

If you are using unity it is recommended you just ignore everything here and use the instructions on the [ecsrx.unity repository](ttps://github.com/grofit/ecsrx.unity).

## Running Examples

If you want to run the examples then just clone it and open examples project in the `src` folder, then run the examples, I will try to add to as the library matures.

There are also a suite of tests which are being expanded as the project grows, it was written with testability in mind.

## Docs

See the docs folder for more information. (This will grow)

[build-status-image]: https://travis-ci.org/grofit/ecsrx.svg
[build-status-url]: https://travis-ci.org/grofit/ecsrx
[gitter-image]: https://badges.gitter.im/grofit/ecsrx.svg
[gitter-url]: https://gitter.im/grofit/ecsrx