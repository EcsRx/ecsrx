using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor
{
    public class SystemExecutor : ISystemExecutor, IDisposable
    {
        private readonly IList<ISystem> _systems;
        private readonly IEnumerable<IConventionalSystemHandler> _conventionalSystemHandlers;
        
        public IEventSystem EventSystem { get; }
        public IPoolManager PoolManager { get; }
        public IEnumerable<ISystem> Systems => _systems;

        public SystemExecutor(IPoolManager poolManager, IEventSystem eventSystem, IEnumerable<IConventionalSystemHandler> conventionalSystemHandlers)
        {
            PoolManager = poolManager;
            EventSystem = eventSystem;
            _conventionalSystemHandlers = conventionalSystemHandlers;

            _systems = new List<ISystem>();
        }
       
        public void RemoveSystem(ISystem system)
        {
            _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority()
                .ForEachRun(x => x.DestroySystem(system));
            
            _systems.Add(system);
        }

        public void AddSystem(ISystem system)
        {
            _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority()
                .ForEachRun(x => x.SetupSystem(system));

            _systems.Remove(system);
        }
        
        public void Dispose()
        {
            _systems.ForEachRun(RemoveSystem);
        }
    }
}