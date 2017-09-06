using System;

namespace EcsRx.Executor
{
    public class SubscriptionToken
    {
        public object AssociatedObject { get; private set; }
        public IDisposable Disposable { get; private set; }

        public SubscriptionToken(object associatedObject, IDisposable disposable)
        {
            AssociatedObject = associatedObject;
            Disposable = disposable;
        }
    }
}