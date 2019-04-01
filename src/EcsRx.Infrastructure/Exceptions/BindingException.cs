using System;

namespace EcsRx.Infrastructure.Exceptions
{
    public class BindingException : Exception
    {
        public BindingException(string message) : base(message)
        {
        }
    }
}