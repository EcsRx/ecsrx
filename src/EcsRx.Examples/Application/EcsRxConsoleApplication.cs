using System.Collections.Generic;
using System.ComponentModel;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Examples.Dependencies;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Accessors;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Pools;
using EcsRx.Reactive;
using EcsRx.Systems;

namespace EcsRx.Examples.Application
{
    public abstract class EcsRxConsoleApplication : EcsRxApplication
    {
        protected override IDependencyContainer DependencyContainer { get; } = new NinjectDependencyContainer();
    }
}