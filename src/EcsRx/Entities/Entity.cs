using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Events;

namespace EcsRx.Entities
{
    public class Entity : IEntity
    {
        private readonly Dictionary<Type, IComponent> _components;

        public IEventSystem EventSystem { get; }

        public Guid Id { get; }
        public IEnumerable<IComponent> Components => _components.Values;

        public Entity(Guid id, IEventSystem eventSystem)
        {
            Id = id;
            EventSystem = eventSystem;
            _components = new Dictionary<Type, IComponent>();
        }

        public IComponent AddComponent(IComponent component)
        {
            _components.Add(component.GetType(), component);
            EventSystem.Publish(new ComponentsAddedEvent(this, new []{component}));
            return component;
        }

        public void AddComponents(params IComponent[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
            { _components.Add(components[i].GetType(), components[i]); }
            
            EventSystem.Publish(new ComponentsAddedEvent(this, components));
        }
        
        public T AddComponent<T>() where T : class, IComponent, new()
        { return (T)AddComponent(new T()); }

        public void RemoveComponent(IComponent component)
        {
            if(!_components.ContainsKey(component.GetType())) { return; }

            if (component is IDisposable disposable)
            {  disposable.Dispose(); }

            _components.Remove(component.GetType());
            EventSystem.Publish(new ComponentsRemovedEvent(this, new []{component}));
        }

        public void RemoveComponent<T>() where T : class, IComponent
        {
            if(!HasComponent<T>()) { return; }

            var component = GetComponent<T>();
            RemoveComponent(component);
        }

        public void RemoveComponents(params IComponent[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
            {
                if(!_components.ContainsKey(components[i].GetType())) { continue; }

                if (components[i] is IDisposable disposable)
                { disposable.Dispose(); }

                _components.Remove(components[i].GetType());
            }
            
            EventSystem.Publish(new ComponentsRemovedEvent(this, components));
        }


        public void RemoveAllComponents()
        {
            var components = Components.ToArray();
            RemoveComponents(components);
        }

        public bool HasComponent<T>() where T : class, IComponent
        { return _components.ContainsKey(typeof(T)); }

        public bool HasComponents(params Type[] componentTypes)
        {
            if(_components.Count == 0)
            { return false; }
            
            for (var index = componentTypes.Length - 1; index >= 0; index--)
            {
                var x = componentTypes[index];
                if (!_components.ContainsKey(x)) return false;
            }

            return true;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            var componentType = typeof(T);
            return (T)GetComponent(componentType);
        }

        public IComponent GetComponent(Type componentType)
        { return _components.TryGetValue(componentType, out var component) ? component : null; }

        public override int GetHashCode()
        { return Id.GetHashCode(); }

        public void Dispose()
        { RemoveAllComponents(); }
    }
}
