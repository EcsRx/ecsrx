using System.Collections.Generic;

namespace EcsRx.Plugins.Persistence.Data
{
    public class ApplicationData
    {
        public IList<EntityCollectionData> EntityCollections { get; set; }
        public string Version { get; set; }
        
        public ApplicationData()
        {
            EntityCollections = new List<EntityCollectionData>();
            Version = "1.0.0";
        }
    }
}