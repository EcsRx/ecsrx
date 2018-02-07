using System;
using System.Collections.Generic;
using EcsRx.Groups;

namespace EcsRx.Groups.Accessors
{
    public class ObservableGroupToken
    {
        public Type[] ComponentTypes { get; private set; }
        public string Pool { get; private set; }

        public ObservableGroupToken(Type[] componentTypes, string pool)
        {
            ComponentTypes = componentTypes;
            Pool = pool;
        }
    }
}