using System;
using System.Collections.Generic;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;

namespace EcsRx.Plugins.Views
{
    public class ViewsPlugin : ISystemsRxPlugin
    {
        public string Name => "Views Plugin";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyContainer container)
        {
            // Nothing needs registering
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
    }
}