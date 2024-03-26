﻿using System;
using System.Collections.Generic;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Infrastructure.Plugins;
using SystemsRx.Systems;
using EcsRx.Plugins.Persistence.Modules;

namespace EcsRx.Plugins.Persistence
{
    public class PersistencePlugin : ISystemsRxPlugin
    {
        public string Name => "Persistence Plugin";
        public Version Version { get; } = new Version("1.0.0");

        public void SetupDependencies(IDependencyRegistry registry)
        {
            registry.LoadModule<LazyDataModule>();
            registry.LoadModule<PersistityModule>();
        }
        
        public IEnumerable<ISystem> GetSystemsForRegistration(IDependencyResolver resolver) => Array.Empty<ISystem>();
    }
}