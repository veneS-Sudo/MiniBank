using System;

namespace Minibank.Core.Domains.Transfers.Services
{
    public class FractionalNumberEditor : IFractionalNumberEditor
    {
        public decimal Round(decimal d, int decimals)
        {
            return Math.Round(d, decimals);
        }
    }
}