using System;
using SystemsRx.Systems;

namespace SystemsRx.Exceptions
{
    public class SystemAlreadyRegisteredException : Exception
    {
        public SystemAlreadyRegisteredException(ISystem system) : base($"System [{system.GetType().Name}] has already been registered")
        {
        }
    }
}