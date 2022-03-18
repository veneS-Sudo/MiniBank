using System;

namespace Minibank.Core.Exceptions.FriendlyException
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        { }
    }
}