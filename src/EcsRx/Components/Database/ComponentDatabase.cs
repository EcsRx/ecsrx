using System.Collections.Generic;
using System.Linq;
using EcsRx.Extensions;

namespace EcsRx.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public int CurrentEntityBounds
        {
            get
            {
                if (EntityComponents.Length == 0)
                { return 0; }

                return EntityComponents[0].Count;
            }
        }

        public List<IComponent>[] EntityComponents { get; private set; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int entitySetupSize = 500)
        {
            ComponentTypeLookup = componentTypeLookup;
            Initialize(entitySetupSize);
        }

        public void Initialize(int entitySetupSize)
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().Values.ToArray();
            EntityComponents = new List<IComponent>[componentTypes.Length];
            AccommodateMoreEntities(entitySetupSize);
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            for (var i = EntityComponents.Length - 1; i >= 0; i--)
            { EntityComponents[i].Insert(newMaxSize, null); }
        }
        
        public IComponent Get(int componentTypeId, int entityId)
        { return EntityComponents[componentTypeId][entityId]; }

        public IEnumerable<IComponent> GetAll(int entityId)
        {
            for (var i = EntityComponents.Length - 1; i >= 0; i--)
            {
                var component = EntityComponents[i][entityId];
                if(component != null) { yield return component; }
            }
        }

        public bool Has(int componentTypeId, int entityId)
        {
            if(EntityComponents[componentTypeId].Count <= entityId)
            { return false; }
            
            return EntityComponents[componentTypeId][entityId] != null;
        }

        public void Add(int componentTypeId, int entityId, IComponent component)
        { EntityComponents[componentTypeId][entityId] = component; }

        public void Remove(int componentTypeId, int entityId)
        { EntityComponents[componentTypeId][entityId] = null; }
        
        public void RemoveAll(int entityId)
        {
            for (var i = EntityComponents.Length - 1; i >= 0; i--)
            { Remove(i, entityId); }
        }
    }
}