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
        public int CurrentEntityBounds
        {
            get
            {
                if (EntityReferenceComponents.Length == 0)
                { return 0; }

                return EntityReferenceComponents[0].Length;
            }
        }

        public bool[] ValueTypeLookups { get; private set; }
        public ValueType[] DefaultValueTypeLookups { get; private set; }
        public IComponent[][] EntityReferenceComponents { get; private set; }
        public Array[] EntityValueComponents { get; private set; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int entitySetupSize = 5000)
        {
            ComponentTypeLookup = componentTypeLookup;
            Initialize(entitySetupSize);
        }
        
        static ValueType CreateDefaultStruct(Type type)
        { return (ValueType)Activator.CreateInstance(type); }

        public Array CreateTypeOnTheFly(Type type, int size)
        {
            var dynamicArray = type.MakeArrayType();
            return (Array)Activator.CreateInstance(dynamicArray, size);
        }

        public void Initialize(int entitySetupSize)
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            EntityReferenceComponents = new IComponent[componentCount][];
            EntityValueComponents = new Array[componentCount];
            ValueTypeLookups = new bool[componentCount];
            DefaultValueTypeLookups = new ValueType[componentCount];

            for (var i = 0; i < EntityReferenceComponents.Length; i++)
            {
                ValueTypeLookups[i] = componentTypes[i].Key.IsValueType;

                if (ValueTypeLookups[i])
                {
                    EntityValueComponents[i] = CreateTypeOnTheFly(componentTypes[i].Key, entitySetupSize);
                    DefaultValueTypeLookups[i] = CreateDefaultStruct(componentTypes[i].Key);
                }
                else
                { EntityReferenceComponents[i] = new IComponent[entitySetupSize]; }
            }            
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            var expandBy = newMaxSize - CurrentEntityBounds;
            for (var i = 0; i < EntityReferenceComponents.Length; i++)
            {
                if(EntityReferenceComponents[i] != null)
                { IListExtensions.ExpandListTo(ref EntityReferenceComponents[i], expandBy); }
                else
                { IListExtensions.ExpandListTo(ref EntityValueComponents[i], expandBy); }
            }
        }
        
        public IComponent Get(int componentTypeId, int entityId)
        { return EntityReferenceComponents[componentTypeId][entityId]; }

        public T GetStruct<T>(int componentTypeId, int entityId) where T : struct
        { return ((T[])EntityValueComponents[componentTypeId])[entityId]; }

        public bool IsComponentStruct(int componentTypeId)
        { return ValueTypeLookups[componentTypeId]; }

        public IReadOnlyList<IComponent> GetComponents(int componentTypeId)
        { return EntityReferenceComponents[componentTypeId]; }

        public IReadOnlyList<T> GetComponentStructs<T>(int componentTypeId) where T : struct
        { return (IReadOnlyList<T>)EntityValueComponents[componentTypeId]; }

        public IEnumerable<IComponent> GetAll(int entityId)
        {
            for (var i = EntityReferenceComponents.Length - 1; i >= 0; i--)
            {
                var component = EntityReferenceComponents[i][entityId];
                if(component != null) { yield return component; }
                
                var valueComponent = ((IComponent[])EntityValueComponents[i])[entityId];
                if(valueComponent != null) { yield return valueComponent; }
            }
        }

        public bool Has(int componentTypeId, int entityId)
        {
            if(EntityReferenceComponents[componentTypeId].Length <= entityId)
            { return false; }

            if (IsComponentStruct(componentTypeId))
            { return !((IList)EntityValueComponents[componentTypeId])[entityId].Equals(DefaultValueTypeLookups[componentTypeId]); }

            return EntityReferenceComponents[componentTypeId][entityId] != null;
        }

        public void Add(int componentTypeId, int entityId, IComponent component)
        {
            if(IsComponentStruct(componentTypeId))
            { ((IList)EntityValueComponents[componentTypeId])[entityId] = component; }
            else
            { EntityReferenceComponents[componentTypeId][entityId] = component; }
        }

        public void Remove(int componentTypeId, int entityId)
        {
            if(IsComponentStruct(componentTypeId))
            { ((IList)EntityValueComponents[componentTypeId])[entityId] = DefaultValueTypeLookups[componentTypeId]; }
            else
            { EntityReferenceComponents[componentTypeId][entityId] = null; }
        }
        
        public void RemoveAll(int entityId)
        {
            for (var i = EntityReferenceComponents.Length - 1; i >= 0; i--)
            { Remove(i, entityId); }
        }
    }
}