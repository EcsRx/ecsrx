using System;

namespace EcsRx.Groups.Observable
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