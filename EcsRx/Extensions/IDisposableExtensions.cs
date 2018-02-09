using System;
using System.Collections.Generic;
using EcsRx.Executor;

namespace EcsRx.Extensions
{
    public static class IDisposableExtensions
    {
        public static void DisposeAll(this IEnumerable<IDisposable> disposables)
        { disposables.ForEachRun(x => x.Dispose()); }

        public static void DisposeAll(this IEnumerable<SubscriptionToken> disposables)
        { disposables.ForEachRun(x => x.Disposable.Dispose()); }
        
        public static void DisposeAll<T>(this IDictionary<T, IDisposable> disposables)
        { disposables.Values.ForEachRun(x => x.Dispose()); }
        
        public static IDisposable AddTo(this IDisposable currentDisposable, ICollection<IDisposable> disposables)
        {
            disposables.Add(currentDisposable);
            return currentDisposable;
        }
        
        public static IDisposable AddTo<T>(this IDisposable currentDisposable, IDictionary<T, IDisposable> disposables, T key)
        {
            disposables.Add(key, currentDisposable);
            return currentDisposable;
        }
    }
}