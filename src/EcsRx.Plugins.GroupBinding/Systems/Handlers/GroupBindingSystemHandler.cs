using System;
using System.Linq;
using System.Reflection;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Systems;
using SystemsRx.Types;
using EcsRx.Collections;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.GroupBinding.Attributes;
using EcsRx.Plugins.GroupBinding.Exceptions;
using EcsRx.Systems;
using EcsRx.Extensions;
using EcsRx.Attributes;

namespace EcsRx.Plugins.GroupBinding.Systems.Handlers
{
    /// <summary>
    /// This will check all ISystem implementations to see if it contains any properties or fields that are
    /// IObservableGroups and if they have an attribute to indicate population from a group
    /// </summary>
    /// <remarks>
    /// The priority is 10 higher than SuperHigh just to make sure it runs before most common systems
    /// </remarks>
    [Priority(PriorityTypes.SuperHigh + 10)]
    public class GroupBindingSystemHandler : IConventionalSystemHandler
    {
        private static readonly Type FromGroupAttributeType = typeof(FromGroupAttribute); 
        private static readonly Type FromComponentsAttributeType = typeof(FromComponentsAttribute); 
        
        public IObservableGroupManager ObservableGroupManager { get; }

        public GroupBindingSystemHandler(IObservableGroupManager observableGroupManager)
        { ObservableGroupManager = observableGroupManager; }

        public bool CanHandleSystem(ISystem system)
        { return true; }

        public Tuple<IGroup, int[]> GetGroupAndAffinityFromAttributeIfAvailable(ISystem system, MemberInfo member)
        {
            var fromGroupAttribute = (FromGroupAttribute)member.GetCustomAttribute(FromGroupAttributeType, true);
            if (fromGroupAttribute != null)
            {
                var possibleGroup = fromGroupAttribute.Group;
                if (possibleGroup != null)
                { return new Tuple<IGroup, int[]>(possibleGroup, system.GetGroupAffinities(member)); }

                if (system is IGroupSystem groupSystem)
                { return new Tuple<IGroup, int[]>(groupSystem.Group, system.GetGroupAffinities(member) ?? groupSystem.GetGroupAffinities()); }

                throw new MissingGroupSystemInterfaceException(system, member);
            }
            
            var fromComponentsAttribute = (FromComponentsAttribute)member.GetCustomAttribute(FromComponentsAttributeType, true);
            if (fromComponentsAttribute != null)
            { return new Tuple<IGroup, int[]>(fromComponentsAttribute.Group, system.GetGroupAffinities(member)); }

            return new Tuple<IGroup, int[]>(null, null);
        }

        public PropertyInfo[] GetApplicableProperties(Type systemType)
        {
            return systemType.GetProperties()
                .Where(x => x.CanWrite && x.PropertyType == typeof(IObservableGroup))
                .ToArray();
        }
        
        public FieldInfo[] GetApplicableFields(Type systemType)
        {
            return systemType.GetFields()
                .Where(x => x.FieldType == typeof(IObservableGroup))
                .ToArray();
        }

        public void ProcessProperty(PropertyInfo property, ISystem system)
        {
            var tuple = GetGroupAndAffinityFromAttributeIfAvailable(system, property);
            var observableGroup = tuple.Item1;
            if (observableGroup == null) { return; }

            property.SetValue(system, ObservableGroupManager.GetObservableGroup(observableGroup, tuple.Item2));
        }

        public void ProcessField(FieldInfo field, ISystem system)
        {
            var tuple = GetGroupAndAffinityFromAttributeIfAvailable(system, field);
            var observableGroup = tuple.Item1;
            if (observableGroup == null) { return; }

            field.SetValue(system, ObservableGroupManager.GetObservableGroup(observableGroup, tuple.Item2));
        }
        
        public void SetupSystem(ISystem system)
        {
            var systemType = system.GetType();
            var observableGroupProperties = GetApplicableProperties(systemType);
            var observableGroupFields = GetApplicableFields(systemType);

            if (observableGroupProperties.Length == 0 && observableGroupFields.Length == 0)
            { return; }

            foreach (var observableGroupProperty in observableGroupProperties)
            { ProcessProperty(observableGroupProperty, system); }
            
            foreach (var observableGroupField in observableGroupFields)
            { ProcessField(observableGroupField, system); }
        }

        public void DestroySystem(ISystem system)
        {}
        
        public void Dispose()
        {}
    }
}