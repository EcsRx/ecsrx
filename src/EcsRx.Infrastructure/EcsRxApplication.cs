using SystemsRx.Infrastructure;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Infrastructure.Modules;

namespace EcsRx.Infrastructure
{
    public abstract class EcsRxApplication : SystemsRxApplication, IEcsRxApplication
    {
        public IEntityDatabase EntityDatabase { get; private set; }
        public IObservableGroupManager ObservableGroupManager { get; private set; }
        
        /// <summary>
        /// Load any modules that your application needs
        /// </summary>
        /// <remarks>
        /// If you wish to use the default setup call through to base, if you wish to stop the default framework
        /// modules loading then do not call base and register your own internal framework module.
        /// </remarks>
        protected override void LoadModules()
        {
            base.LoadModules();
            Container.LoadModule(new EcsRxInfrastructureModule());
        }

        /// <summary>
        /// Resolve any dependencies the application needs
        /// </summary>
        /// <remarks>By default it will setup IEntityDatabase, IObservableGroupManager and base class dependencies</remarks>
        protected override void ResolveApplicationDependencies()
        {
            base.ResolveApplicationDependencies();
            EntityDatabase = Container.Resolve<IEntityDatabase>();
            ObservableGroupManager = Container.Resolve<IObservableGroupManager>();
        }
    }
}