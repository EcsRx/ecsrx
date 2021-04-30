using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsRx.Groups;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Modules
{
    public class ComputedModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            var namedHealthGroup = container.ResolveObservableGroup(new Group(typeof(HasHealthComponent), typeof(HasNameComponent)));
            var computedHealthGroup = new LowestHealthComputedGroup(namedHealthGroup);
            container.Bind<ILowestHealthComputedGroup>(x => x.ToInstance(computedHealthGroup));
        }
    }
}