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

        public static void ExpandListTo<T>(this List<T> list, int amountToAdd) where T : class
        {
            for (var i = 0; i < amountToAdd; i++)
            { list.Add(null); }
        }
        
        public static void ExpandListTo<T>(ref T[] array, int amountToAdd)
        {
            var arrayType = array.GetType();
            var newEntries = (T[])Activator.CreateInstance(arrayType, array.Length + amountToAdd);               
            array.CopyTo(newEntries, 0);
            array = newEntries;
        }
        
        public static void ExpandListTo(ref Array array, int amountToAdd)
        {
            var arrayType = array.GetType();
            var newEntries = (Array)Activator.CreateInstance(arrayType, array.Length + amountToAdd);               
            array.CopyTo(newEntries, 0);
            array = newEntries;
        }
    }
}