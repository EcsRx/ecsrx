using System;
using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Plugins.Persistence.Modules;
using EcsRx.Systems;

namespace EcsRx.Plugins.Persistence
{
    public class PersistencePlugin : IEcsRxPlugin
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