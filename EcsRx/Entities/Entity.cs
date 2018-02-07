using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Events;
using EcsRx.Extensions;

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
            EventSystem.Publish(new ComponentAddedEvent(this, component));
            return component;
        }

        public T AddComponent<T>() where T : class, IComponent, new()
        { return (T)AddComponent(new T()); }

        public void RemoveComponent(IComponent component)
        {
            if(!_components.ContainsKey(component.GetType())) { return; }

            var disposable = component as IDisposable;
            if (disposable != null)
            {  disposable.Dispose(); }

            _components.Remove(component.GetType());
            EventSystem.Publish(new ComponentRemovedEvent(this, component));
        }

        public void RemoveComponent<T>() where T : class, IComponent
        {
            if(!HasComponent<T>()) { return; }

            var component = GetComponent<T>();
            RemoveComponent(component);
        }

        public void RemoveAllComponents()
        {
            var components = Components.ToArray();
            components.ForEachRun(RemoveComponent);
        }

        public bool HasComponent<T>() where T : class, IComponent
        { return _components.ContainsKey(typeof(T)); }

        public bool HasComponents(params Type[] componentTypes)
        {
            if(_components.Count == 0)
            { return false; }

            //return componentTypes.All(x => _components.ContainsKey(x));
            
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
        {
            IComponent component;
            return _components.TryGetValue(componentType, out component) ? component : null;
        }

        public void Dispose()
        { RemoveAllComponents(); }
    }
}
