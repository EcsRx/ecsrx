using EcsRx.Groups;
using EcsRx.Systems;
using SystemsRx.Attributes;

namespace EcsRx.Tests.Systems.PriorityScenarios
{
    [Priority(-1)]
    public class LowerThanDefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
}