using SystemsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    [Priority(-1)]
    public class LowerThanDefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
}