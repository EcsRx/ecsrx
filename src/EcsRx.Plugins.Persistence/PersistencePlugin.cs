using System;
using System.Collections.Generic;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using SystemsRx.Infrastucture.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.Persistence.Modules;

namespace EcsRx.Plugins.Persistence
{
    public class PersistencePlugin : ISystemsRxPlugin
    {
        public string Name => "Persistence Plugin";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyContainer container)
        {
            container.LoadModule<LazyDataModule>();
            container.LoadModule<PersistityModule>();
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}