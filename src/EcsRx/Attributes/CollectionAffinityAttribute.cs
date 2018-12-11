using System;
using EcsRx.Collections;

namespace EcsRx.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionAffinityAttribute : Attribute
    {
        public string CollectionName { get; }

        public CollectionAffinityAttribute(string collectionName = EntityCollectionManager.DefaultPoolName)
        {
            CollectionName = collectionName;
        }
    }
}