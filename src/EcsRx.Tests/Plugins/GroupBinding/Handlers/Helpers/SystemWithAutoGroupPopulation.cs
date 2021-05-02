using SystemsRx.Systems;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.GroupBinding.Attributes;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class SystemWithAutoGroupPopulation : ISystem, IGroupSystem
    {
        public IGroup Group => new TestGroupA();
        
        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup ObservableGroupA { get; set; }
        
        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup IgnoredObservableGroup { get; }
        
        [FromComponents(typeof(TestComponentTwo))]
        public IObservableGroup ObservableGroupB { get; set; }
        
        public int IgnoredProperty { get; set; }

        [FromGroup]
        public IObservableGroup ObservableGroupC;
        
        public int IgnoredField;
    }
}