namespace Minibank.Core.Domains.Transfers.Services
{
    public interface IFractionalNumberEditor
    {
        decimal Round(decimal d, int decimals);
    }
}