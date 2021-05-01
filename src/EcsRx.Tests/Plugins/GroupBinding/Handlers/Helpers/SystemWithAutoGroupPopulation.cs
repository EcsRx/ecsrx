using System;
using SystemsRx.Systems;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.GroupBinding.Attributes;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.SystemsRx.Handlers.Helpers
{
    public class TestGroupA : IGroup
    {
        public Type[] RequiredComponents { get; set; } = {typeof(TestComponentOne)};
        public Type[] ExcludedComponents { get; set; } = new Type[0];
    }
    
    public class SystemWithAutoGroupPopulation : ISystem, IGroupSystem
    {
        public IGroup Group => new TestGroupA();
        
        [FromGroup(typeof(TestGroupA))]
        public IObservableGroup ObservableGroupA { get; set; }
        
        [FromComponents(typeof(TestComponentTwo))]
        public IObservableGroup ObservableGroupB { get; set; }
        
        public int IgnoredProperty { get; set; }

        [FromGroup]
        public IObservableGroup ObservableGroupC;
        
        public int IgnoredField;
    }
}