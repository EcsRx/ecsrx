using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.PerformanceTests.Extensions
{
    public static class IEnumerableExtensions
    {
        private static readonly Random _random = new Random();
     
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var shuffled = list.ToArray();
            var n = shuffled.Length;  
            while (n > 1) {  
                n--;  
                var k = _random.Next(n + 1);  
                var value = shuffled[k];  
                shuffled[k] = shuffled[n];  
                shuffled[n] = value;  
            }
            return shuffled;
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> list, int amount)
        { return list.Shuffle().Take(amount); }
        
    }
}