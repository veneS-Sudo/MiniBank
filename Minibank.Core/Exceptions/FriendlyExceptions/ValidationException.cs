using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        { }
    }
}