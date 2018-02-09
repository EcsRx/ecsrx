using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Reactive;
using EcsRx.Systems;

namespace EcsRx.Examples.Application
{
    /// <summary>
    /// This here is like the bootstrapper, as this example does not use dependency injection
    /// it is all hardcoded to be setup with the built in types, but provides a quick way
    /// for the examples to be written
    /// </summary>
    public abstract class EcsRxApplication
    {
        public ISystemExecutor SystemExecutor { get; }
        public IEventSystem EventSystem { get; }
        public IPoolManager PoolManager { get; }

        protected EcsRxApplication()
        {
            EventSystem = new EventSystem(new MessageBroker());

            var entityFactory = new DefaultEntityFactory(EventSystem);
            var poolFactory = new DefaultPoolFactory(entityFactory, EventSystem);
            var groupAccessorFactory = new DefaultObservableObservableGroupFactory(EventSystem);
            PoolManager = new PoolManager(EventSystem, poolFactory, groupAccessorFactory);


            var reactsToEntityHandler = new ReactToEntitySystemHandler(PoolManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(PoolManager);
            var reactsToDataHandler = new ReactToDataSystemHandler(PoolManager);
            var manualSystemHandler = new ManualSystemHandler(PoolManager);
            var setupHandler = new SetupSystemHandler(PoolManager);

            var conventionalSystems = new List<IConventionalSystemHandler<ISystem>>
            {
                setupHandler,
                
            }
            
            SystemExecutor = new SystemExecutor(PoolManager, EventSystem, reactsToEntityHandler,
                reactsToGroupHandler, setupHandler, reactsToDataHandler, manualSystemHandler);
        }

        public abstract void StartApplication();
    }
}