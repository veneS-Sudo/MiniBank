using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class ParametersValidationException : Exception
    {
        public ParametersValidationException(string message) : base(message)
        { }
    }
}