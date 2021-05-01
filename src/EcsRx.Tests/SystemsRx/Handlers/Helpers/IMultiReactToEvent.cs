using SystemsRx.Systems.Conventional;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.SystemsRx.Handlers.Helpers
{
    public interface ITestMultiReactToEvent : IReactToEventSystem<ComplexObject>, IReactToEventSystem<int>
    {
        
    }
}