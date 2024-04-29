using Bogus;
using RestJsonProvider.Models;

namespace RestJsonProvider.Generators
{
    public class FakeDataGenerator
    {
        public IEnumerable<BankAccount> GenrateBankAccounts()
        {
            return new Faker<BankAccount>().Rules((f, x) =>
            {
                x.Name = f.Finance.AccountName();
                x.Iban = f.Finance.Iban();
                x.Debt = f.Finance.Random.UInt(100, 100000);
                x.Balance = f.Finance.Random.UInt(100, 100000);
                x.IsLocked = f.Random.Bool();
                DateTime cardDatetime = f.Date.Future(yearsToGoForward: 17, refDate: DateTime.Now);
                x.CreditCard = new CreditCard
                {
                    CreditCardExpireMonth = f.Date.Future(yearsToGoForward: 17, refDate: DateTime.Now).Month,
                    CreditCardExpireYear = cardDatetime.Year,
                    Number = f.Finance.CreditCardNumber(),
                    Ccv = f.Finance.CreditCardCvv(),
                };

                if (x.Balance - x.Debt < 0 || x.IsLocked)
                {
                    x.ToTakeOutaLoan = false;
                }
                else
                {
                    x.ToTakeOutaLoan = true;

                }
            }).Generate(100);
        }

    }
}
