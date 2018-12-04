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

                return EntityReferenceComponents[0].Count;
            }
        }

        public bool[] ValueTypeLookups { get; private set; }
        public List<IComponent>[] EntityReferenceComponents { get; private set; }
        public IList[] EntityValueComponents { get; private set; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public ComponentDatabase(IComponentTypeLookup componentTypeLookup, int entitySetupSize = 500)
        {
            ComponentTypeLookup = componentTypeLookup;
            Initialize(entitySetupSize);
        }

        public IList CreateTypeOnTheFly(Type type)
        {
            var dynamicList = typeof(List<>).MakeGenericType(type);
            return (IList)Activator.CreateInstance(dynamicList);
        }

        public void Initialize(int entitySetupSize)
        {
            var componentTypes = ComponentTypeLookup.GetAllComponentTypes().ToArray();
            var componentCount = componentTypes.Length;
            EntityReferenceComponents = new List<IComponent>[componentCount];
            EntityValueComponents = new IList[componentCount];
            ValueTypeLookups = new bool[componentCount];

            for (var i = 0; i < EntityReferenceComponents.Length; i++)
            {
                ValueTypeLookups[i] = componentTypes[i].Key.IsValueType;

                if (ValueTypeLookups[i])
                { EntityValueComponents[i] = CreateTypeOnTheFly(componentTypes[i].Key); }
                else
                { EntityReferenceComponents[i] = new List<IComponent>(); }
            }
            
            AccommodateMoreEntities(entitySetupSize);
        }
        
        public void AccommodateMoreEntities(int newMaxSize)
        {
            Console.WriteLine("EXPANDING " + newMaxSize);
            var expandBy = newMaxSize - CurrentEntityBounds;
            for (var i = 0; i < EntityReferenceComponents.Length; i++)
            {
                if(EntityReferenceComponents[i] != null)
                { EntityReferenceComponents[i].ExpandListTo<IComponent>(expandBy); }
                else
                { EntityValueComponents[i].ExpandListTo(expandBy); }
            }
            Console.WriteLine("EXPANDED");
        }
        
        public IComponent Get(int componentTypeId, int entityId)
        { return EntityReferenceComponents[componentTypeId][entityId]; }

        public T GetStruct<T>(int componentTypeId, int entityId) where T : struct
        { return (T)EntityValueComponents[componentTypeId][entityId]; }

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
                
                var valueComponent = EntityValueComponents[i][entityId];
                if(valueComponent != null) { yield return (IComponent)valueComponent; }
            }
        }

        public bool Has(int componentTypeId, int entityId)
        {
            if(EntityReferenceComponents[componentTypeId].Count <= entityId)
            { return false; }

            if (IsComponentStruct(componentTypeId))
            { return !EntityValueComponents[componentTypeId][entityId].Equals(-1); }

            return EntityReferenceComponents[componentTypeId][entityId] != null;
        }

        public void Add(int componentTypeId, int entityId, IComponent component)
        {
            if(IsComponentStruct(componentTypeId))
            { EntityValueComponents[componentTypeId][entityId] = (ValueType)component; }
            else
            { EntityReferenceComponents[componentTypeId][entityId] = component; }
        }

        public void Remove(int componentTypeId, int entityId)
        {
            if(IsComponentStruct(componentTypeId))
            { EntityValueComponents[componentTypeId][entityId] = -1; }
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