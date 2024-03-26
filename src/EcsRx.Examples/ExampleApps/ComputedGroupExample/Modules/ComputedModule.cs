using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsRx.Groups;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Modules
{
    public class ComputedModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<ILowestHealthComputedGroup>(x => x.ToMethod(y =>
            {
                var namedHealthGroup = y.ResolveObservableGroup(new Group(typeof(HasHealthComponent), typeof(HasNameComponent)));
                return new LowestHealthComputedGroup(namedHealthGroup);
            }));
        }
    }
}