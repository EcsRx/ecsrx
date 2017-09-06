using System;
using System.Collections.Generic;
using EcsRx.Groups;

namespace EcsRx.Groups.Accessors
{
    public class GroupAccessorToken
    {
        public Type[] ComponentTypes { get; private set; }
        public string Pool { get; private set; }

        public GroupAccessorToken(Type[] componentTypes, string pool)
        {
            ComponentTypes = componentTypes;
            Pool = pool;
        }
    }
}