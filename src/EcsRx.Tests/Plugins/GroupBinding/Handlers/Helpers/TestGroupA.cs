using System;
using EcsRx.Groups;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Plugins.GroupBinding.Handlers.Helpers
{
    public class TestGroupA : IGroup
    {
        public Type[] RequiredComponents { get; set; } = {typeof(TestComponentOne)};
        public Type[] ExcludedComponents { get; set; } = Array.Empty<Type>();
    }
}