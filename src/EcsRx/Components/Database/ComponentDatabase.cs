using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;

namespace EcsRx.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public int CurrentEntityBounds { get; private set; }
        
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public ExpandingArray[] EntityComponents { get; private set; }
        public BitArray[] EntityComponentsLookups { get; private set; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int entitySetupSize = 5000)
        {
            ComponentTypeLookup = componentTypeLookup;
            Initialize(entitySetupSize);
        }
        
        public void Initialize(int entitySetupSize)
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            EntityComponents = new ExpandingArray[componentCount];
            EntityComponentsLookups = new BitArray[componentCount];
            CurrentEntityBounds = entitySetupSize;

            for (var i = 0; i < componentCount; i++)
            {
                EntityComponents[i] = new ExpandingArray(componentTypes[i].Key, entitySetupSize);
                EntityComponentsLookups[i] = new BitArray(entitySetupSize);
            }            
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            var expandBy = newMaxSize - CurrentEntityBounds;
            for (var i = 0; i < EntityComponents.Length; i++)
            {
                EntityComponents[i].Expand(expandBy);
                EntityComponentsLookups[i] = EntityComponentsLookups[i].ExpandListTo(expandBy);
            }
            CurrentEntityBounds = newMaxSize;
        }

        public T Get<T>(int componentTypeId, int entityId) where T : IComponent
        { return EntityComponents[componentTypeId].GetItem<T>(entityId); }

        public T[] GetComponents<T>(int componentTypeId) where T : IComponent
        { return EntityComponents[componentTypeId].GetArray<T>(); }

        public IEnumerable<IComponent> GetAll(int entityId)
        {
            for (var i = EntityComponents.Length - 1; i >= 0; i--)
            {
                if (EntityComponentsLookups[i][entityId])
                { yield return (IComponent)EntityComponents[i].GetItem(entityId); }
            }
        }

        public bool Has(int componentTypeId, int entityId)
        { return EntityComponentsLookups[componentTypeId][entityId]; }

        public void Set<T>(int componentTypeId, int entityId, T component) where T : IComponent
        {
            EntityComponents[componentTypeId].SetItem(entityId, component);
            EntityComponentsLookups[componentTypeId][entityId] = true;
        }

        public void Remove(int componentTypeId, int entityId)
        {
            if (!ComponentTypeLookup.IsComponentStruct(componentTypeId))
            { EntityComponents[componentTypeId].SetItem(entityId, null); }
            EntityComponentsLookups[componentTypeId][entityId] = false;
        }
        
        public void RemoveAll(int entityId)
        {
            for (var i = EntityComponents.Length - 1; i >= 0; i--)
            { Remove(i, entityId); }
        }
    }
}