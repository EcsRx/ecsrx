using System;
using System.Collections.Generic;
using System.Reactive;
using EcsRx.Groups.Accessors;

namespace EcsRx.Groups.Filtration
{
    public abstract class CacheableGroupAccessorFilter<T> : IGroupAccessorFilter<T>, IDisposable
    {
        private IDisposable _triggerSubscription;
        private bool _needsUpdate = true;

        protected IEnumerable<T> FilteredCache { get; set; }
        protected abstract IObservable<Unit> TriggerOnChange();
        protected abstract IEnumerable<T> FilterQuery();

        public IGroupAccessor GroupAccessor { get; private set; }

        protected CacheableGroupAccessorFilter(IGroupAccessor groupAccessor)
        {
            GroupAccessor = groupAccessor;
            SetupTriggers();
        }

        private void SetupTriggers()
        { _triggerSubscription = TriggerOnChange().Subscribe(x => _needsUpdate = true); }

        public IEnumerable<T> Filter()
        {
            if (_needsUpdate)
            {
                FilteredCache = FilterQuery();
                _needsUpdate = false;
            }

            return FilteredCache;
        }

        public void Dispose()
        { _triggerSubscription.Dispose(); }
    }
}