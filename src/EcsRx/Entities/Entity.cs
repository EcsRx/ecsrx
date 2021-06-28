using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;
using SystemsRx.MicroRx.Subjects;
using IComponent = EcsRx.Components.IComponent;

namespace EcsRx.Entities
{
    public class Entity : IEntity
    {
        public static readonly int NotAllocated = -1;
        
        public IObservable<int[]> ComponentsAdded => _onComponentsAdded;
        public IObservable<int[]> ComponentsRemoving => _onComponentsRemoving;
        public IObservable<int[]> ComponentsRemoved => _onComponentsRemoved;
        
        private readonly Subject<int[]> _onComponentsAdded;
        private readonly Subject<int[]> _onComponentsRemoving;
        private readonly Subject<int[]> _onComponentsRemoved;
        
        public int Id { get; }
        
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase ComponentDatabase { get; }
        
        public int[] InternalComponentAllocations { get; }
        public IReadOnlyList<int> ComponentAllocations => InternalComponentAllocations;

        public IEnumerable<IComponent> Components
        {
            get
            {
                for (var componentTypeId = 0; componentTypeId < InternalComponentAllocations.Length; componentTypeId++)
                {
                    if(InternalComponentAllocations[componentTypeId] != NotAllocated)
                    { yield return GetComponent(componentTypeId);}
                }
            }
        }
        
        public Entity(int id, IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            Id = id;
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            
            var totalComponentCount = componentTypeLookup.AllComponentTypeIds.Length;
            InternalComponentAllocations = new int[totalComponentCount];
            
            _onComponentsAdded = new Subject<int[]>();
            _onComponentsRemoving = new Subject<int[]>();
            _onComponentsRemoved = new Subject<int[]>();
            
            EmptyAllAllocations();
        }

        public void EmptyAllAllocations()
        {
            for (var i = 0; i < InternalComponentAllocations.Length; i++)
            { InternalComponentAllocations[i] = NotAllocated; }
        }
        
        public void AddComponents(IReadOnlyList<IComponent> components)
        {
            var componentTypeIds = new int[components.Count];
            for (var i = 0; i < components.Count; i++)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(components[i].GetType());
                var allocationId = ComponentDatabase.Allocate(componentTypeId);
                InternalComponentAllocations[componentTypeId] = allocationId;
                ComponentDatabase.Set(componentTypeId, allocationId, components[i]);
                componentTypeIds[i] = componentTypeId;
            }
            
            _onComponentsAdded.OnNext(componentTypeIds);
        }

        public ref T AddComponent<T>(int componentTypeId) where T : IComponent, new()
        {
            var defaultComponent = ComponentTypeLookup.CreateDefault<T>();
            var allocationId = ComponentDatabase.Allocate(componentTypeId);
            InternalComponentAllocations[componentTypeId] = allocationId;
            ComponentDatabase.Set(componentTypeId, allocationId, defaultComponent);
            
            _onComponentsAdded.OnNext(new []{componentTypeId});
            
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationId);
        }
        
        public void UpdateComponent<T>(int componentTypeId, T newValue) where T : struct, IComponent
        {
            var allocationId = InternalComponentAllocations[componentTypeId];
            ComponentDatabase.Set(componentTypeId, allocationId, newValue);
        }
        
        public void RemoveComponents(params Type[] componentTypes)
        {
            var componentTypeIds = ComponentTypeLookup.GetComponentTypes(componentTypes);
            RemoveComponents(componentTypeIds);
        }
        
        public void RemoveComponents(IReadOnlyList<int> componentsTypeIds)
        {
            var sanitisedComponentsIds = componentsTypeIds.Where(HasComponent).ToArray();
            if(sanitisedComponentsIds.Length == 0) { return; }
            
            _onComponentsRemoving.OnNext(sanitisedComponentsIds);

            for (var i = 0; i < sanitisedComponentsIds.Length; i++)
            {
                var componentId = sanitisedComponentsIds[i];
                var allocationIndex = InternalComponentAllocations[componentId];
                ComponentDatabase.Remove(componentId, allocationIndex);
                InternalComponentAllocations[componentId] = NotAllocated;
            }
            
            _onComponentsRemoved.OnNext(sanitisedComponentsIds);
        }

        public void RemoveAllComponents()
        { RemoveComponents(ComponentTypeLookup.AllComponentTypeIds); }

        public bool HasComponent(Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentType(componentType);
            return HasComponent(componentTypeId);
        }

        public bool HasComponent(int componentTypeId)
        { return InternalComponentAllocations[componentTypeId] != NotAllocated; }

        public IComponent GetComponent(Type componentType)
        {
            var componentTypeId = ComponentTypeLookup.GetComponentType(componentType);
            return GetComponent(componentTypeId);
        }

        public IComponent GetComponent(int componentTypeId)
        {
            var allocationIndex = InternalComponentAllocations[componentTypeId];
            return ComponentDatabase.Get(allocationIndex, componentTypeId);
        }

        public ref T GetComponent<T>(int componentTypeId) where T : IComponent
        {
            var allocationIndex = InternalComponentAllocations[componentTypeId];
            return ref ComponentDatabase.GetRef<T>(componentTypeId, allocationIndex);
        }

        public override int GetHashCode()
        { return Id; }

        public void Dispose()
        {
            RemoveAllComponents();
            _onComponentsAdded.Dispose();
            _onComponentsRemoving.Dispose();
            _onComponentsRemoved.Dispose();
        }
    }
}
