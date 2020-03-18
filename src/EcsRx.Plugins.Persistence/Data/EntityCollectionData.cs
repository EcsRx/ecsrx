using System.Collections.Generic;

namespace EcsRx.Plugins.Persistence.Data
{
    public class EntityCollectionData
    {
        public int CollectionId { get; set; }
        public List<EntityData> Entities { get; set; }

        public EntityCollectionData()
        {
            Entities = new List<EntityData>();
        }
    }
}