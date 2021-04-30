using System;

namespace SystemsRx.Infrastructure.Exceptions
{
    public class BindingException : Exception
    {
        public BindingException(string message) : base(message)
        {
        }
    }
}