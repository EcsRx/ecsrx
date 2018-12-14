using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;

namespace EcsRx.Components.Database
{
    public class ComponentDatabase : IComponentDatabase
    {
        public int CurrentEntityBounds { get; private set; }
        
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public IList<IComponent>[] EntityReferenceComponents { get; private set; }
        public Array[] EntityValueComponents { get; private set; }
        public BitArray[] EntityValueComponentsLookups { get; private set; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int entitySetupSize = 5000)
        {
            ComponentTypeLookup = componentTypeLookup;
            Initialize(entitySetupSize);
        }
        
        public Array CreateTypeOnTheFly(Type type, int size)
        {
            var dynamicArray = type.MakeArrayType();
            return (Array)Activator.CreateInstance(dynamicArray, size);
        }

        public void Initialize(int entitySetupSize)
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            EntityReferenceComponents = new IList<IComponent>[componentCount];
            EntityValueComponents = new Array[componentCount];
            EntityValueComponentsLookups = new BitArray[componentCount];
            CurrentEntityBounds = entitySetupSize;

            for (var i = 0; i < componentCount; i++)
            {
                if (ComponentTypeLookup.IsComponentStruct(i))
                { EntityValueComponents[i] = CreateTypeOnTheFly(componentTypes[i].Key, entitySetupSize); }
                else
                { EntityReferenceComponents[i] = new IComponent[entitySetupSize]; }
                
                EntityValueComponentsLookups[i] = new BitArray(entitySetupSize);
            }            
        }
        
        public void ExpandUntypedArray(ref Array array, int amountToAdd)
        {
            var arrayType = array.GetType();
            var newEntries = (Array)Activator.CreateInstance(arrayType, array.Length + amountToAdd);               
            array.CopyTo(newEntries, 0);
            array = newEntries;
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            var expandBy = newMaxSize - CurrentEntityBounds;
            for (var i = 0; i < EntityReferenceComponents.Length; i++)
            {
                if (EntityReferenceComponents[i] != null)
                {
                    var reference = (IComponent[]) EntityReferenceComponents[i];
                    EntityReferenceComponents[i] = reference.ExpandListTo(expandBy);
                }
                else
                { ExpandUntypedArray(ref EntityValueComponents[i], expandBy); }
                
                EntityValueComponentsLookups[i] = EntityValueComponentsLookups[i].ExpandListTo(expandBy);
            }
            CurrentEntityBounds = newMaxSize;
        }

        public T Get<T>(int componentTypeId, int entityId) where T : IComponent
        {
            if (ComponentTypeLookup.IsComponentStruct(componentTypeId))
            { return ((IReadOnlyList<T>) EntityValueComponents[componentTypeId])[entityId]; }
            return (T) EntityReferenceComponents[componentTypeId][entityId];
        }

        public IReadOnlyList<T> GetComponents<T>(int componentTypeId) where T : IComponent
        {
            if (ComponentTypeLookup.IsComponentStruct(componentTypeId))
            { return (IReadOnlyList<T>) EntityValueComponents[componentTypeId]; }
            
            return (IReadOnlyList<T>)EntityReferenceComponents[componentTypeId];
        }

        public IEnumerable<IComponent> GetAll(int entityId)
        {
            for (var i = EntityReferenceComponents.Length - 1; i >= 0; i--)
            {
                if (EntityReferenceComponents[i] != null)
                {
                    var component = EntityReferenceComponents[i][entityId];
                    if(component != null) { yield return component; }
                }
                else
                {
                    if (EntityValueComponentsLookups[i][entityId])
                    { yield return (IComponent)((IList) EntityValueComponents[i])[entityId]; }
                }
            }
        }

        public bool Has(int componentTypeId, int entityId)
        {
            if (CurrentEntityBounds <= entityId) { return false; }
            return EntityValueComponentsLookups[componentTypeId][entityId];
        }

        public void Set<T>(int componentTypeId, int entityId, T component) where T : IComponent
        {
            if (ComponentTypeLookup.IsComponentStruct(componentTypeId))
            { ((IList)EntityValueComponents[componentTypeId])[entityId] = component; }
            else
            { EntityReferenceComponents[componentTypeId][entityId] = component; }
            
            EntityValueComponentsLookups[componentTypeId][entityId] = true;
        }

        public void Remove(int componentTypeId, int entityId)
        {
            if (!ComponentTypeLookup.IsComponentStruct(componentTypeId))
            { EntityReferenceComponents[componentTypeId][entityId] = null; }
            
            EntityValueComponentsLookups[componentTypeId][entityId] = false;
        }
        
        public void RemoveAll(int entityId)
        {
            for (var i = EntityReferenceComponents.Length - 1; i >= 0; i--)
            { Remove(i, entityId); }
        }
    }
}