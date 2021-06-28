using System;
using System.Collections.Generic;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.GroupBinding.Systems.Handlers;

namespace EcsRx.Plugins.GroupBinding
{
    public class GroupBindingsPlugin : ISystemsRxPlugin
    {
        public string Name => "Group Bindings";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyContainer container)
        { container.Bind<IConventionalSystemHandler, GroupBindingSystemHandler>(); }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
    }
}