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
        private static Type FromGroupAttributeType = typeof(FromGroupAttribute); 
        private static Type FromComponentsAttributeType = typeof(FromComponentsAttribute); 
        
        public IObservableGroupManager ObservableGroupManager { get; }

        public GroupBindingSystemHandler(IObservableGroupManager observableGroupManager)
        { ObservableGroupManager = observableGroupManager; }

        public bool CanHandleSystem(ISystem system)
        { return true; }

        public IGroup GetGroupAttributeIfAvailable(ISystem system, MemberInfo member)
        {
            var fromGroupAttributes = member.GetCustomAttributes(FromGroupAttributeType, true);
            if (fromGroupAttributes.Length > 0)
            {
                var possibleGroup = ((FromGroupAttribute)fromGroupAttributes.First()).Group;
                if(possibleGroup != null) { return possibleGroup; }

                if (system is IGroupSystem groupSystem)
                { return groupSystem.Group; }

                throw new MissingGroupSystemInterfaceException(system, member);
            }
            
            var fromComponentsAttributes = member.GetCustomAttributes(FromComponentsAttributeType, true);
            if (fromComponentsAttributes.Length > 0)
            { return ((FromComponentsAttribute)fromComponentsAttributes.First()).Group; }

            return null;
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
            var possibleGroup = GetGroupAttributeIfAvailable(system, property);
            if (possibleGroup == null) { return; }
                
            var observableGroup = ObservableGroupManager.GetObservableGroup(possibleGroup);
            property.SetValue(system, observableGroup);
        }

        public void ProcessField(FieldInfo field, ISystem system)
        {
            var possibleGroup = GetGroupAttributeIfAvailable(system, field);
            if (possibleGroup == null) { return; }
                
            var observableGroup = ObservableGroupManager.GetObservableGroup(possibleGroup);
            field.SetValue(system, observableGroup);
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