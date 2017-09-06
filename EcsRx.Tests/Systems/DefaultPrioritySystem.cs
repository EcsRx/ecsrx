using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems
{
    public class DefaultPrioritySystem : ISystem
    {
        public IGroup TargetGroup { get { return null; } }
    }
}