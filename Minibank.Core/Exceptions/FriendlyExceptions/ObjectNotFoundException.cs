using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException(string message) : base(message)
        { }
    }
}