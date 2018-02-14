using System;
using System.Collections.Generic;
using EcsRx.Groups;

namespace EcsRx.Groups.Accessors
{
    public class ObservableGroupToken
    {
        public Type[] ComponentTypes { get; }
        public string Pool { get; }

        public ObservableGroupToken(Type[] componentTypes, string pool)
        {
            ComponentTypes = componentTypes;
            Pool = pool;
        }
    }
}