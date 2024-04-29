namespace RestJsonProvider.Models
{
    public class BankAccount
    {
        private string? _name;
        public required string Name
        {
            get
            {
                return $"---{_name}---";

            }
            set { _name = value; }
        }
        public required string Iban { get; set; }
        public decimal Debt { get; set; }
        public decimal Balance { get; set; }
        public bool IsLocked { get; set; }
        public required CreditCard CreditCard { get; set; }
        public bool ToTakeOutaLoan { get; set; }
    }
}
