using System;
using System.Reflection;

namespace EcsRx.Components.Lookups
{
    public class StructDefaulter : IStructDefaulter
    {
        public ValueType[] DefaultValueTypeLookups { get; }

        public ValueType GetDefault(int index) => DefaultValueTypeLookups[index];
        public bool IsDefault<T>(T value, int index) => value.Equals(GetDefault(index));
        
        public ValueType GenerateDefault(Type type)
        {
            var defaultObject = Activator.CreateInstance(type);
            var fields = type.GetFields(BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.IsStatic) { continue; }

                if(field.FieldType.IsPrimitive)
                { field.SetValue(defaultObject, byte.MinValue+1); }           
            }

            return (ValueType)defaultObject;
        }
    }
}