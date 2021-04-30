using SystemsRx.Attributes;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    [Priority(-100)]
    public class LowestPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    [Priority(-101)]
    public class LowestPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        
        public void Setup(IEntity entity) {}
    }
    
    [Priority(-102)]
    public class LowestPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntity entity){}
        public void Setup(IEntity entity){}
    }
}