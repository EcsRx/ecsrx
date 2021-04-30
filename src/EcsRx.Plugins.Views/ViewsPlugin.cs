using System;
using System.Collections.Generic;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Plugins;
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
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}