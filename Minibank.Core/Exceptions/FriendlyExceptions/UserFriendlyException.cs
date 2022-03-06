using System;

namespace Minibank.Core.Exceptions.FriendlyException
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        { }
    }
}