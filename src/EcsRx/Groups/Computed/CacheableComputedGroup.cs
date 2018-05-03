using System;
using System.Collections.Generic;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;

namespace EcsRx.Groups.Computed
{
    public abstract class CacheableComputedGroup<T> : IComputedGroup<T>, IDisposable
    {
        private IDisposable _triggerSubscription;
        private bool _needsUpdate = true;

        protected IEnumerable<T> FilteredCache { get; set; }
        protected abstract IObservable<bool> TriggerOnChange();
        protected abstract IEnumerable<T> FilterQuery();

        public IObservableGroup ObservableGroup { get; }

        protected CacheableComputedGroup(IObservableGroup observableGroup)
        {
            ObservableGroup = observableGroup;
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