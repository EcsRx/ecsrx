using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Exceptions;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Systems;

namespace SystemsRx.Executor
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
       
        public bool HasSystem(ISystem system)
        { return _systems.Contains(system); }
        
        public void RemoveSystem(ISystem system)
        {
            var applicableHandlers = _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority();

            foreach(var handler in applicableHandlers)
            { handler.DestroySystem(system); }
            
            _systems.Remove(system);
        }

        public void AddSystem(ISystem system)
        {
            if(HasSystem(system))
            { throw new SystemAlreadyRegisteredException(system); }
            
            var applicableHandlers = _conventionalSystemHandlers
                .Where(x => x.CanHandleSystem(system))
                .OrderByPriority();

            foreach(var handler in applicableHandlers)
            { handler.SetupSystem(system); }

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