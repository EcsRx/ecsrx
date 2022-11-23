using System;
using System.Collections.Generic;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.ReactiveSystems.Handlers;

namespace EcsRx.Plugins.ReactiveSystems
{
    public class ReactiveSystemsPlugin : ISystemsRxPlugin
    {
        public string Name => "Reactive Systems";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyContainer container)
        { container.LoadModule<ReactiveSystemsModule>(); }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
    }
}