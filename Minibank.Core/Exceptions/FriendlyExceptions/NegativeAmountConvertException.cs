using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class NegativeAmountConvertException : Exception
    {
        public NegativeAmountConvertException(string message) : base(message)
        { }
    }
}