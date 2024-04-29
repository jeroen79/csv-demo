namespace RestJsonProvider.Models
{
    public class BankAccount
    {
        public required string Name { get; set; }
        public required string Iban { get; set; }
        public decimal Debt { get; set; }
        public decimal Balance { get; set; }
        public bool IsLocked { get; set; }
        public CreditCard? CreditCard { get; set; }
        public bool ToTakeOutaLoan { get; set; }
    }
}
