using System;
using EcsRx.Collections.Database;

namespace EcsRx.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class CollectionAffinityAttribute : Attribute
    {
        public int[] CollectionIds { get; }

        public CollectionAffinityAttribute(int collectionId = EntityCollectionLookups.DefaultCollectionId)
        { CollectionIds = new []{collectionId}; }

        public CollectionAffinityAttribute(params int[] collectionIds)
        { CollectionIds = collectionIds; }
    }
}