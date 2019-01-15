using System;
using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Plugins.Batching.Accessors;
using EcsRx.Plugins.Batching.Factories;
using EcsRx.Systems;

namespace EcsRx.Plugins.Batching
{
    public class BatchPlugin : IEcsRxPlugin
    {
        public string Name => "Batching";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyContainer container)
        {
            container.Bind<IBatchBuilderFactory, BatchBuilderFactory>(x => x.AsSingleton());
            container.Bind<IReferenceBatchBuilderFactory, ReferenceBatchBuilderFactory>(x => x.AsSingleton());
            container.Bind<IBatchManager, BatchManager>(x => x.AsSingleton());
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => new ISystem[0];
    }
}