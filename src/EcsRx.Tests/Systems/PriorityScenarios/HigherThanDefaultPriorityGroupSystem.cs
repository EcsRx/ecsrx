using EcsRx.Groups;
using EcsRx.Systems;
using SystemsRx.Attributes;

namespace EcsRx.Tests.Systems.PriorityScenarios
{
    [Priority(1)]
    public class HigherThanDefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
}