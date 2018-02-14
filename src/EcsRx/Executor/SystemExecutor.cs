using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Systems;

namespace EcsRx.Executor
{
    public class SystemExecutor : ISystemExecutor, IDisposable
    {
        public readonly IList<ISystem> _systems;
        public readonly IEnumerable<IConventionalSystemHandler> _conventionalSystemHandlers;
        
        public IEnumerable<ISystem> Systems => _systems;

        public SystemExecutor(IEnumerable<IConventionalSystemHandler> conventionalSystemHandlers)
        {
            _conventionalSystemHandlers = conventionalSystemHandlers;

            _systems = new List<ISystem>();
        }
       
        public void RemoveSystem(ISystem system)
        {
            _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority()
                .ForEachRun(x => x.DestroySystem(system));
            
            _systems.Remove(system);
        }

        public void AddSystem(ISystem system)
        {
            _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority()
                .ForEachRun(x => x.SetupSystem(system));

            _systems.Add(system);
        }
        
        public void Dispose()
        {
            for(var i= _systems.Count - 1; i >= 0; i--)
            { RemoveSystem(_systems[i]); }
            
            _systems.Clear();
            _conventionalSystemHandlers.DisposeAll();
        }
    }
}