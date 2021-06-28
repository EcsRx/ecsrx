using System;
using System.Collections.Generic;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.Batching.Accessors;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Plugins.Batching
{
    public class BatchPlugin : ISystemsRxPlugin
    {
        public string Name => "Batching";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyContainer container)
        {
            container.Bind<IBatchBuilderFactory, BatchBuilderFactory>(x => x.AsSingleton());
            container.Bind<IReferenceBatchBuilderFactory, ReferenceBatchBuilderFactory>(x => x.AsSingleton());
            container.Bind<IBatchManager, BatchManager>(x => x.AsSingleton());
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container) => Array.Empty<ISystem>();
    }
}