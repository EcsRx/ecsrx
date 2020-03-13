using System.Collections.Generic;

namespace EcsRx.Plugins.Persistence.Data
{
    public class EntityCollectionData
    {
        public int CollectionId { get; set; }
        public IList<EntityData> Entities { get; set; }
        public string Version { get; set; }

        public EntityCollectionData()
        {
            Entities = new List<EntityData>();
            Version = "1.0.0";
        }
    }
}