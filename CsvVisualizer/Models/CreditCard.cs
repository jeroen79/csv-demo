namespace RestJsonProvider.Models
{
    public class CreditCard
    {
        public required string Number { get; set; }
        public required int CreditCardExpireMonth { get; set; }
        public required int CreditCardExpireYear { get; set; }
        public required string Ccv { get; set; }
    }
}
