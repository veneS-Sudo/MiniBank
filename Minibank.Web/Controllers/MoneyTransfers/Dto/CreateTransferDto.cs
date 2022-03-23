namespace Minibank.Web.Controllers.MoneyTransfers.Dto
{
    public class CreateTransferDto
    {
        public double Amount { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}