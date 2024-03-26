using System;
using System.Collections.Generic;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems
{
    public class ReactiveSystemsPlugin : ISystemsRxPlugin
    {
        public string Name => "Reactive Systems";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyRegistry registry)
        { registry.LoadModule<ReactiveSystemsModule>(); }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}