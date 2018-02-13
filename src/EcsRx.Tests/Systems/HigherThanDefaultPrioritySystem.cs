using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    [Priority(100)]
    public class HigherThanDefaultPrioritySystem : ISystem
    {
        public IGroup TargetGroup => null;
    }
}