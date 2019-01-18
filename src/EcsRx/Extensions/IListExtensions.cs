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
        
        public static T[] ExpandListTo<T>(this T[] array, int amountToAdd)
        {
            var arrayType = array.GetType();
            var newEntries = (T[])Activator.CreateInstance(arrayType, array.Length + amountToAdd);               
            array.CopyTo(newEntries, 0);
            return newEntries;
        }
        
        public static BitArray ExpandListTo(this BitArray array, int amountToAdd)
        {
            var newArray = new BitArray(array.Length + amountToAdd);
            for (var i = 0; i < array.Length; i++)
            { newArray[i] = array[i]; }

            return newArray;
        }
    }
}