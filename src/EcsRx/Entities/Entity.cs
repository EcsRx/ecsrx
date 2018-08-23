using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.MicroRx;
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
        public IEnumerable<IComponent> Components => ComponentRepository.GetAll(Id);

        public Entity(int id, IComponentRepository componentRepository)
        {
            Id = id;
            ComponentRepository = componentRepository;
            componentRepository.ExpandDatabaseIfNeeded(id);
            
            _onComponentsAdded = new Subject<int[]>();
            _onComponentsRemoving = new Subject<int[]>();
            _onComponentsRemoved = new Subject<int[]>();
        }

        public void AddComponents(params IComponent[] components)
        {
            var componentTypeIds = new int[components.Length];
            for (var i = components.Length - 1; i >= 0; i--)
            { componentTypeIds[i] = ComponentRepository.Add(Id, components[i]); }            
            
            _onComponentsAdded.OnNext(componentTypeIds);
        }
        
        public void RemoveComponents(params Type[] componentTypes)
        {
            var componentTypeIds = ComponentRepository.GetTypesFor(componentTypes);
            RemoveComponents(componentTypeIds);
        }

        public void RemoveComponents(params int[] componentsTypeIds)
        {
            var sanitisedComponentsIds = componentsTypeIds.Where(HasComponent).ToArray();
            if(sanitisedComponentsIds.Length == 0) { return; }
            
            _onComponentsRemoving.OnNext(sanitisedComponentsIds);
            
            for (var i = 0; i < sanitisedComponentsIds.Length; i++)
            { ComponentRepository.Remove(Id, sanitisedComponentsIds[i]); }
            
            _onComponentsRemoved.OnNext(sanitisedComponentsIds);
        }

        public void RemoveAllComponents()
        {
            var componentTypes = Components.Select(x => x.GetType()).ToArray();
            var componentTypeIds = ComponentRepository.GetTypesFor(componentTypes);
            RemoveComponents(componentTypeIds);
        }
       
        public bool HasComponent(Type componentType)
        { return ComponentRepository.Has(Id, componentType); }

        public bool HasComponent(int componentTypeId)
        { return ComponentRepository.Has(Id, componentTypeId); }

        public IComponent GetComponent(Type componentType)
        { return ComponentRepository.Get(Id, componentType); }
        
        public IComponent GetComponent(int componentTypeId)
        { return ComponentRepository.Get(Id, componentTypeId); }

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
