using System;
using EcsRx.Systems;

namespace EcsRx.Exceptions
{
    public class SystemAlreadyRegisteredException : Exception
    {
        public SystemAlreadyRegisteredException(ISystem system) : base($"System [{system.GetType().Name}] has already been registered")
        {
        }
    }
}