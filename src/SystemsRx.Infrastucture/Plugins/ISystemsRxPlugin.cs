using System;
using System.Collections.Generic;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Systems;

namespace SystemsRx.Infrastucture.Plugins
{
    public interface ISystemsRxPlugin
    {
        string Name { get; }
        Version Version { get; }

        void SetupDependencies(IDependencyContainer container);
        IEnumerable<ISystem> GetSystemsForRegistration(IDependencyContainer container);
    }
}