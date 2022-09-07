using System;

namespace Minibank.Core.Exceptions.FriendlyExceptions
{
    public class TransferNotCompletedException : Exception
    {
        public TransferNotCompletedException(string message) : base(message)
        {}
    }
}