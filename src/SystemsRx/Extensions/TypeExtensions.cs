using System;
using System.Linq;

namespace SystemsRx.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetMatchingInterfaceGenericTypes(this Type type, Type genericType)
        {
            return type
                .GetInterfaces()
                .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
        }
    }
}