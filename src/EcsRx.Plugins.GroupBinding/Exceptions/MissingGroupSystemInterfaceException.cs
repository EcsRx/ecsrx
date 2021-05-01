using System;
using System.Reflection;
using SystemsRx.Systems;

namespace EcsRx.Plugins.GroupBinding.Exceptions
{
    public class MissingGroupSystemInterfaceException : Exception
    {
        public ISystem System { get; }
        public MemberInfo Member { get; }

        public MissingGroupSystemInterfaceException(ISystem system, MemberInfo member) 
            : base($"{member.Name} GroupFrom attribute cannot find an IGroupSystem on {system.GetType().Name}")
        {
            System = system;
            Member = member;
        }
    }
}