using System;
using System.Collections;
using System.Collections.Generic;

namespace EcsRx.Extensions
{
    public static class IListExtensions
    {
        public static void RemoveAllFrom<T>(this IList<T> list, IEnumerable<T> elementsToRemove)
        {
            foreach (var element in elementsToRemove)
            { list.Remove(element); }
        }

        public static void ExpandListTo<T>(this IList<T> list, int amountToAdd) where T : class
        {
            for (var i = 0; i < amountToAdd; i++)
            { list.Add(null); }
        }
        
        public static void ExpandListTo(this IList list, int amountToAdd)
        {
            var internalType = list.GetType().GetGenericArguments()[0];
            var arrayType = internalType.MakeArrayType();
            var newEntries = (IList)Activator.CreateInstance(arrayType, amountToAdd);
            for (var i = 0; i < amountToAdd; i++)
            { list.Add(newEntries[i]); }
        }
    }
}