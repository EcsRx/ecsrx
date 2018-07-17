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
        public IObservable<IComponent[]> ComponentsAdded => _onComponentsAdded;
        public IObservable<IComponent[]> ComponentsRemoving => _onComponentsRemoving;
        public IObservable<IComponent[]> ComponentsRemoved => _onComponentsRemoved;
        
        private readonly Subject<IComponent[]> _onComponentsAdded;
        private readonly Subject<IComponent[]> _onComponentsRemoving;
        private readonly Subject<IComponent[]> _onComponentsRemoved;
        
        public int Id { get; }
        public IComponentRepository ComponentRepository { get; }
        public IEnumerable<IComponent> Components => ComponentRepository.GetAll(Id);

        public Entity(int id, IComponentRepository componentRepository)
        {
            Id = id;
            ComponentRepository = componentRepository;
            componentRepository.ExpandDatabaseIfNeeded(id);
            
            _onComponentsAdded = new Subject<IComponent[]>();
            _onComponentsRemoving = new Subject<IComponent[]>();
            _onComponentsRemoved = new Subject<IComponent[]>();
        }

        public IComponent AddComponent(IComponent component)
        {
            ComponentRepository.Add(Id, component);
            _onComponentsAdded.OnNext(new []{component});
            return component;
        }

        public void AddComponents(params IComponent[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
            { ComponentRepository.Add(Id, components[i]); }
            
            _onComponentsAdded.OnNext(components);
        }
        
        public T AddComponent<T>() where T : class, IComponent, new()
        { return (T)AddComponent(new T()); }

        public void RemoveComponent(IComponent component)
        { RemoveComponents(component); }

        public void RemoveComponent<T>() where T : class, IComponent
        { RemoveComponents(default(T)); }

        public void RemoveComponents(params IComponent[] components)
        {
            _onComponentsRemoving.OnNext(components);
            
            for (var i = 0; i < components.Length; i++)
            { ComponentRepository.Remove(Id, components[i].GetType()); }
            
            _onComponentsRemoved.OnNext(components);
        }

        public void RemoveAllComponents()
        { RemoveComponents(Components.ToArray()); }

        public bool HasComponent<T>() where T : class, IComponent
        { return ComponentRepository.Has(Id, typeof(T)); }

        public bool HasAllComponents(params Type[] componentTypes)
        {
            for (var index = componentTypes.Length - 1; index >= 0; index--)
            {
                if(!ComponentRepository.Has(Id, componentTypes[index]))
                { return false; }
            }

            return true;
        }
        
        public bool HasAnyComponents(params Type[] componentTypes)
        {
            for (var index = componentTypes.Length - 1; index >= 0; index--)
            {
                if(ComponentRepository.Has(Id, componentTypes[index]))
                { return true; }
            }

            return false;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            var componentType = typeof(T);
            return (T)GetComponent(componentType);
        }

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
