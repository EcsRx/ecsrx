using System.Collections.Generic;
using EcsRx.Components.Lookups;
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
            var componentCount = ComponentTypeLookup.GetAllComponentTypes().Count;
            EntityComponents = new List<IComponent>[componentCount];
            
            for(var i=0;i<EntityComponents.Length;i++)
            { EntityComponents[i] = new List<IComponent>(); }
            
            AccommodateMoreEntities(entitySetupSize);
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            var expandBy = newMaxSize - CurrentEntityBounds;
            for (var i=0;i<EntityComponents.Length;i++)
            { EntityComponents[i].ExpandListTo(expandBy); }
        }
        
        public IComponent Get(int componentTypeId, int entityId)
        { return EntityComponents[componentTypeId][entityId]; }

        public IReadOnlyList<IComponent> GetComponents(int componentTypeId)
        { return EntityComponents[componentTypeId]; }

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