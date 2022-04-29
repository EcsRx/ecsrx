using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Infrastructure.Modules;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Modules;
using SystemsRx.Infrastructure.Ninject;

namespace EcsRx.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31, warmupCount: 1, invocationCount: 1)]
    [MemoryDiagnoser]
    public abstract class EcsRxBenchmark
    {
        public IDependencyContainer Container { get; }
        public IEntityDatabase EntityDatabase { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        public IObservableGroupManager ObservableGroupManager { get; }

        protected EcsRxBenchmark()
        {
            Container = new NinjectDependencyContainer();
            Container.LoadModule(new FrameworkModule());
            Container.LoadModule(new EcsRxInfrastructureModule());
            
            EntityDatabase = Container.Resolve<IEntityDatabase>();
            ObservableGroupManager = Container.Resolve<IObservableGroupManager>();
            ComponentDatabase = Container.Resolve<IComponentDatabase>();
            ComponentTypeLookup = Container.Resolve<IComponentTypeLookup>();
        }

        [GlobalSetup]
        public abstract void Setup();

        [GlobalCleanup]
        public abstract void Cleanup();
    }
}