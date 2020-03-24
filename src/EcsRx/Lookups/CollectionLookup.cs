using System.Collections.ObjectModel;
using EcsRx.Collections;
using EcsRx.Collections.Entity;

namespace EcsRx.Lookups
{
    public class CollectionLookup : KeyedCollection<int, IEntityCollection>
    {
        protected override int GetKeyForItem(IEntityCollection item) => item.Id;

        public IEntityCollection GetByIndex(int index) => Items[index];
    }
}