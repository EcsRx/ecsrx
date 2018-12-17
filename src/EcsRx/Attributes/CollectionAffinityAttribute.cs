using System;
using EcsRx.Collections;

namespace EcsRx.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionAffinityAttribute : Attribute
    {
        public string[] CollectionNames { get; }

        public CollectionAffinityAttribute(string collectionName = EntityCollectionManager.DefaultPoolName)
        { CollectionNames = new []{collectionName}; }

        public CollectionAffinityAttribute(params string[] collectionNames)
        { CollectionNames = collectionNames; }
    }
}