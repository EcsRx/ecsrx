using System.Collections.ObjectModel;
using EcsRx.Entities;

namespace EcsRx.Lookups
{
    public class EntityLookup : KeyedCollection<int, IEntity>
    {
        protected override int GetKeyForItem(IEntity item) => item.Id;

        public IEntity GetByIndex(int index) => Items[index];
    }
}