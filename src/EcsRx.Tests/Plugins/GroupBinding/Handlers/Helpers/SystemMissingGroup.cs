using SystemsRx.Systems;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.GroupBinding.Attributes;

namespace EcsRx.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemMissingGroup : ISystem
    {
        [FromGroup()]
        public IObservableGroup ObservableGroupA { get; set; }
    }
}