using System;
using System.Collections.Generic;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using SystemsRx.Infrastucture.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.ReactiveSystems.Handlers;

namespace EcsRx.Plugins.ReactiveSystems
{
    public class ReactiveSystemsPlugin : ISystemsRxPlugin
    {
        public string Name => "Reactive Systems";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyContainer container)
        {
            container.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            container.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            container.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
        }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}