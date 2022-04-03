namespace Minibank.Web.Controllers.MoneyTransfers.Dto
{
    public class CreateMoneyTransferDto
    {
        public double Amount { get; set; }
        public string FromBankAccountId { get; set; }
        public string ToBankAccountId { get; set; }
    }
}