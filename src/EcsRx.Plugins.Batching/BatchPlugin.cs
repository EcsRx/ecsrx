using System;
using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Plugins.Batching.Collections;
using EcsRx.Systems;

namespace EcsRx.Plugins.Batching
{
    public class BatchPlugin : IEcsRxPlugin
    {
        public string Name => "Reactive Systems";
        public Version Version { get; } = new Version("1.0.0");
        
        public void SetupDependencies(IDependencyContainer container)
        {
            container.Bind<IBatchManager, BatchManager>();
        }

        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}