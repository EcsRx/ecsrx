using System;

namespace SystemsRx.Infrastucture.Exceptions
{
    public class BindingException : Exception
    {
        public BindingException(string message) : base(message)
        {
        }
    }
}