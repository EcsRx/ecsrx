using System;
using EcsRx.Examples.Custom.Components;
using EcsRx.Groups;

namespace EcsRx.Examples.Custom.Groups
{
    class MessageGroup : IGroup
    {
        public Type[] RequiredComponents { get;  } = {typeof(FirstComponent) };

        public Type[] ExcludedComponents { get; } = new Type[0];
    }
}
