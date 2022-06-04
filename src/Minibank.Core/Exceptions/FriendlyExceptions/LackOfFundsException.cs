using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class LackOfFundsException : Exception
    {
        public LackOfFundsException(string message) : base(message)
        { }
    }
}