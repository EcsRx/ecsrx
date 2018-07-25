using System;
using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class GroupBuilder
    {
        private List<Type> _withComponents;
        private List<Type> _withoutComponents;
        private Predicate<IEntity> _predicate;

        public GroupBuilder()
        {
            _withComponents = new List<Type>();
            _withoutComponents = new List<Type>();
        }

        public GroupBuilder Create()
        {
            _withComponents = new List<Type>();
            _withoutComponents = new List<Type>();
            return this;
        }

        public GroupBuilder WithComponent<T>() where T : class, IComponent
        {
            _withComponents.Add(typeof(T));
            return this;
        }
        
        public GroupBuilder WithoutComponent<T>() where T : class, IComponent
        {
            _withoutComponents.Add(typeof(T));
            return this;
        }


        public GroupBuilder WithPredicate(Predicate<IEntity> predicate)
        {
            _predicate = predicate;
            return this;
        }

        public IGroup Build()
        { return new Group(_predicate, _withComponents, _withoutComponents); }
    }
}