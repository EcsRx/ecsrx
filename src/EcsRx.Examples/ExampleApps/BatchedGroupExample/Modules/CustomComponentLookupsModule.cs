using System;
using System.Collections.Generic;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Components.Lookups;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Lookups;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Modules
{
    public class CustomComponentLookupsModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Unbind<IComponentTypeLookup>();
            var explicitTypeLookups = new Dictionary<Type, int>
            {
                {typeof(NameComponent), ComponentLookupTypes.NameComponentId},
                {typeof(PositionComponent), ComponentLookupTypes.PositionComponentId},
                {typeof(MovementSpeedComponent), ComponentLookupTypes.MovementSpeedComponentId}
            };
            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);
            container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});
        }
    }
}