using System;
using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Systems;

namespace EcsRx.Plugins.Computeds
{
    public class ComputedsPlugin : IEcsRxPlugin
    {
        public string Name => "Computeds";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyContainer container)
        {
            // Nothing needs registering
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}