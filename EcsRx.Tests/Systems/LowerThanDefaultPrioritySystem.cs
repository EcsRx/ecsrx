using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    [Priority(-1)]
    public class LowerThanDefaultPrioritySystem : ISystem
    {
        public IGroup TargetGroup => null;
    }
}