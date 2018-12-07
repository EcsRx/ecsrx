using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Entities
{
    public class Entity : IEntity
    {
        public IObservable<int[]> ComponentsAdded => _onComponentsAdded;
        public IObservable<int[]> ComponentsRemoving => _onComponentsRemoving;
        public IObservable<int[]> ComponentsRemoved => _onComponentsRemoved;
        
        private readonly Subject<int[]> _onComponentsAdded;
        private readonly Subject<int[]> _onComponentsRemoving;
        private readonly Subject<int[]> _onComponentsRemoved;
        
        public int Id { get; }
        
        public IComponentRepository ComponentRepository { get; }
        public List<int> ActiveComponents { get; }

        public IEnumerable<IComponent> Components
        {
            get
            {
                for (var i = 0; i < ActiveComponents.Count; i++)
                { yield return GetComponent(ActiveComponents[i]); }
            }
        }
        
        public Entity(int id, IComponentRepository componentRepository)
        {
            Id = id;
            ComponentRepository = componentRepository;
            ActiveComponents = new List<int>();
            componentRepository.ExpandDatabaseIfNeeded(id);
            
            _onComponentsAdded = new Subject<int[]>();
            _onComponentsRemoving = new Subject<int[]>();
            _onComponentsRemoved = new Subject<int[]>();
        }

        public void AddComponents(params IComponent[] components)
        { AddComponents((IReadOnlyList<IComponent>)components); }
        
        public void AddComponents(IReadOnlyList<IComponent> components)
        {
            var componentTypeIds = new int[components.Count];
            for (var i = 0; i < components.Count; i++)
            {
                componentTypeIds[i] = ComponentRepository.Add(Id, components[i]);
                ActiveComponents.Add(componentTypeIds[i]);
            }
            
            _onComponentsAdded.OnNext(componentTypeIds);
        }

        public T AddComponent<T>(int componentTypeId) where T : IComponent, new()
        {
            var component = ComponentRepository.Create<T>(Id, componentTypeId);
            ActiveComponents.Add(componentTypeId);
            return component;
        }
        
        public void RemoveComponents(params Type[] componentTypes)
        {
            var componentTypeIds = ComponentRepository.ComponentTypeLookup.GetComponentTypes(componentTypes);
            RemoveComponents(componentTypeIds);
        }

        public void RemoveComponents(params int[] componentsTypeIds)
        { RemoveComponents((IReadOnlyList<int>)componentsTypeIds); }
        
        public void RemoveComponents(IReadOnlyList<int> componentsTypeIds)
        {
            var sanitisedComponentsIds = componentsTypeIds.Where(HasComponent).ToArray();
            if(sanitisedComponentsIds.Length == 0) { return; }
            
            _onComponentsRemoving.OnNext(sanitisedComponentsIds);

            for (var i = 0; i < sanitisedComponentsIds.Length; i++)
            {
                ComponentRepository.Remove(Id, sanitisedComponentsIds[i]);
                ActiveComponents.Remove(sanitisedComponentsIds[i]);
            }
            
            _onComponentsRemoved.OnNext(sanitisedComponentsIds);
        }

        public void RemoveAllComponents()
        { RemoveComponents(ActiveComponents); }

        public bool HasComponent(Type componentType)
        {
            var componentTypeId = ComponentRepository.ComponentTypeLookup.GetComponentType(componentType);
            return HasComponent(componentTypeId);
        }

        public bool HasComponent(int componentTypeId)
        { return ComponentRepository.Has(Id, componentTypeId); }

        public IComponent GetComponent(Type componentType)
        {
            var componentTypeId = ComponentRepository.ComponentTypeLookup.GetComponentType(componentType);
            return GetComponent(componentTypeId);
        }
        
        public IComponent GetComponent(int componentTypeId)
        { return ComponentRepository.Get(Id, componentTypeId); }
        
        public T GetComponent<T>(int componentTypeId) where T : IComponent
        { return ComponentRepository.Get<T>(Id, componentTypeId); }

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
