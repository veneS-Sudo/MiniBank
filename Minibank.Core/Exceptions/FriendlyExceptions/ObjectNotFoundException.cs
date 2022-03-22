using System;

namespace Minibank.Core.Exceptions.FriendlyException
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException(string message) : base(message)
        { }
    }
}