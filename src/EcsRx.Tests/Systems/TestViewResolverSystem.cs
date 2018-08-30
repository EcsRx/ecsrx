using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Tests.Models;
using EcsRx.Views.Systems;

namespace EcsRx.Tests.Systems
{
    public class TestViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => new Group(typeof(TestComponentOne));
        
        public void Teardown(IEntity entity)
        {
            var testComponent = entity.GetComponent<TestComponentOne>();
            testComponent.Data = null;
        }

        public void Setup(IEntity entity)
        {
            var testComponent = entity.GetComponent<TestComponentOne>();
            testComponent.Data = "woop";
        }
    }
}