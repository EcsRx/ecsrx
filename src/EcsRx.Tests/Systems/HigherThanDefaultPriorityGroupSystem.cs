using SystemsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    [Priority(1)]
    public class HigherThanDefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
}