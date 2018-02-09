using System;

namespace EcsRx.Executor
{
    public class SubscriptionToken
    {
        public object AssociatedObject { get; }
        public IDisposable Disposable { get; }

        public SubscriptionToken(object associatedObject, IDisposable disposable)
        {
            AssociatedObject = associatedObject;
            Disposable = disposable;
        }
    }
}