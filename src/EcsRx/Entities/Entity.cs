using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Polyfills;

namespace EcsRx.Entities
{
    public class Entity : IEntity
    {
        public IObservable<Type[]> ComponentsAdded => _onComponentsAdded;
        public IObservable<Type[]> ComponentsRemoving => _onComponentsRemoving;
        public IObservable<Type[]> ComponentsRemoved => _onComponentsRemoved;
        
        private readonly Subject<Type[]> _onComponentsAdded;
        private readonly Subject<Type[]> _onComponentsRemoving;
        private readonly Subject<Type[]> _onComponentsRemoved;
        
        public int Id { get; }
        public IComponentRepository ComponentRepository { get; }
        public IEnumerable<IComponent> Components => ComponentRepository.GetAll(Id);

        public Entity(int id, IComponentRepository componentRepository)
        {
            Id = id;
            ComponentRepository = componentRepository;
            componentRepository.ExpandDatabaseIfNeeded(id);
            
            _onComponentsAdded = new Subject<Type[]>();
            _onComponentsRemoving = new Subject<Type[]>();
            _onComponentsRemoved = new Subject<Type[]>();
        }

        public void AddComponents(params IComponent[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
            { ComponentRepository.Add(Id, components[i]); }
            
            _onComponentsAdded.OnNext(components.Select(x => x.GetType()).ToArray());
        }
        
        public void RemoveComponents(params Type[] componentTypes)
        {
            var sanitisedComponents = componentTypes.Where(HasComponent).ToArray();
            if(sanitisedComponents.Length == 0) { return; }
            
            _onComponentsRemoving.OnNext(sanitisedComponents);
            
            for (var i = 0; i < sanitisedComponents.Length; i++)
            { ComponentRepository.Remove(Id, sanitisedComponents[i]); }
            
            _onComponentsRemoved.OnNext(sanitisedComponents);
        }

        public void RemoveAllComponents()
        { RemoveComponents(Components.Select(x => x.GetType()).ToArray()); }
       
        public bool HasComponent(Type componentType)
        { return ComponentRepository.Has(Id, componentType); }
        
        public IComponent GetComponent(Type componentType)
        { return ComponentRepository.Get(Id, componentType); }

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
