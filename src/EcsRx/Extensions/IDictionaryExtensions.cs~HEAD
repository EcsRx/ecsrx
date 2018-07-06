using System;
using System.Collections.Generic;

namespace EcsRx.Extensions
{
    public static class IDictionaryExtensions
    {       
        public static void RemoveAndDispose<T>(this IDictionary<T, IDisposable> disposables, T key)
        {
            disposables[key].Dispose();
            disposables.Remove(key);
        }
    }
}