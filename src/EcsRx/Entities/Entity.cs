using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Events;

namespace EcsRx.Entities
{
    public class Entity : IEntity
    {
        private readonly Dictionary<int, IComponent> _components;

        public IEventSystem EventSystem { get; }

        public Guid Id { get; }
        public IEnumerable<IComponent> Components => _components.Values;

        public Entity(Guid id, IEventSystem eventSystem)
        {
            Id = id;
            EventSystem = eventSystem;
            _components = new Dictionary<int, IComponent>();
        }

        public IComponent AddComponent(IComponent component)
        {
            _components.Add(component.GetType().GetHashCode(), component);
            EventSystem.Publish(new ComponentsAddedEvent(this, new []{component}));
            return component;
        }

        public void AddComponents(params IComponent[] components)
        {
            EventSystem.Publish(new ComponentsBeforeAddedEvent(this, components));

            for (var i = components.Length - 1; i >= 0; i--)
            { _components.Add(components[i].GetType().GetHashCode(), components[i]); }
            
            EventSystem.Publish(new ComponentsAddedEvent(this, components));
        }
        
        public T AddComponent<T>() where T : class, IComponent, new()
        { return (T)AddComponent(new T()); }

        public void RemoveComponent(IComponent component)
        { RemoveComponents(component); }

        public void RemoveComponent<T>() where T : class, IComponent
        {
            if(!HasComponent<T>()) { return; }

            var component = GetComponent<T>();
            RemoveComponents(component);
        }

        public void RemoveComponents(params IComponent[] components)
        {
            EventSystem.Publish(new ComponentsBeforeRemovedEvent(this, components));

            for (var i = components.Length - 1; i >= 0; i--)
            {
                if(!_components.ContainsKey(components[i].GetType().GetHashCode())) { continue; }

                if (components[i] is IDisposable disposable)
                { disposable.Dispose(); }

                _components.Remove(components[i].GetType().GetHashCode());
            }
            
            EventSystem.Publish(new ComponentsRemovedEvent(this, components));
        }


        public void RemoveAllComponents()
        {
            var components = Components.ToArray();
            RemoveComponents(components);
        }

        public bool HasComponent<T>() where T : class, IComponent
        { return _components.ContainsKey(typeof(T).GetHashCode()); }

        public bool HasComponents(params Type[] componentTypes)
        {
            if(_components.Count == 0)
            { return false; }
            
            for (var index = componentTypes.Length - 1; index >= 0; index--)
            {
                var x = componentTypes[index];
                if (!_components.ContainsKey(x.GetHashCode())) return false;
            }

            return true;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            var componentType = typeof(T);
            return (T)GetComponent(componentType);
        }

        public IComponent GetComponent(Type componentType)
        { return _components.TryGetValue(componentType.GetHashCode(), out var component) ? component : null; }

        public override int GetHashCode()
        { return Id.GetHashCode(); }

        public void Dispose()
        { RemoveAllComponents(); }
    }
}
