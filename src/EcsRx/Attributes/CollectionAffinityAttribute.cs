using System;
using EcsRx.Collections;

namespace EcsRx.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionAffinityAttribute : Attribute
    {
        public int[] CollectionIds { get; }

        public CollectionAffinityAttribute(int collectionId = PoolLookups.DefaultPoolId)
        { CollectionIds = new []{collectionId}; }

        public CollectionAffinityAttribute(params int[] collectionIds)
        { CollectionIds = collectionIds; }
    }
}