using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Infrastructure.Modules;
using EcsRx.Plugins.ReactiveSystems;
using SystemsRx.Executor;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Modules;
using SystemsRx.Infrastructure.Ninject;

namespace EcsRx.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31, warmupCount: 1, invocationCount: 1, iterationCount: 10)]
    [MemoryDiagnoser]
    [MarkdownExporter]
    public abstract class EcsRxBenchmark
    {
        public IDependencyRegistry DependencyRegistry { get; }
        public IDependencyResolver DependencyResolver { get; }
        public IEntityDatabase EntityDatabase { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public ISystemExecutor SystemExecutor { get; }
        
        public IObservableGroupManager ObservableGroupManager { get; }

        protected EcsRxBenchmark()
        {
            DependencyRegistry = new NinjectDependencyRegistry();
            DependencyRegistry.LoadModule(new FrameworkModule());
            DependencyRegistry.LoadModule(new EcsRxInfrastructureModule());
            DependencyRegistry.LoadModule(new ReactiveSystemsModule());
            DependencyResolver = DependencyRegistry.BuildResolver();
            
            EntityDatabase = DependencyResolver.Resolve<IEntityDatabase>();
            ObservableGroupManager = DependencyResolver.Resolve<IObservableGroupManager>();
            ComponentDatabase = DependencyResolver.Resolve<IComponentDatabase>();
            ComponentTypeLookup = DependencyResolver.Resolve<IComponentTypeLookup>();
            SystemExecutor = DependencyResolver.Resolve<ISystemExecutor>();
        }

        [GlobalSetup]
        public abstract void Setup();

        [GlobalCleanup]
        public abstract void Cleanup();
    }
}