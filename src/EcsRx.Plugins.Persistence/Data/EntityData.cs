using System.Collections.Generic;
using EcsRx.Components;

namespace EcsRx.Plugins.Persistence.Data
{
    public class EntityData
    {
        public int EntityId { get; set; }
        public IList<IComponent> Components { get; set; }
        public string Version { get; set; }

        public EntityData()
        {
            Components = new List<IComponent>();
            Version = "1.0.0";
        }
    }
}