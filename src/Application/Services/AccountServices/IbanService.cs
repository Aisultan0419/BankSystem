using System.Text;
using Application.Interfaces.Repositories;
using Domain.Enums;
using System.Numerics;
using Application.Interfaces.Services.Accounts;
namespace Application.Services.AccountServices
{
    public class IbanService : IIbanService
    {
        private readonly IAccountRepository _accountRepository;
        private const string countryCode = "KZ";
        private const string bankCode = "AISS";

        public IbanService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }            

        public async Task<string> GetIban(AccountType accountType, Guid Id)
        {
            string uniqueId = ((BigInteger.Abs(new BigInteger(Id.ToByteArray()))).ToString())[^4..];
            string orderNumber = await _accountRepository.GetOrderNumber() 
                ?? throw new InvalidOperationException("Failed to generate account order number");
            string accountTypeFormatted = ((int)accountType).ToString("D3");
            string uniqueNumber = string.Concat(accountTypeFormatted, uniqueId, orderNumber);

            string letters = countryCode + bankCode;

            string digitalizedCode = Digitalization(letters);

            string checkDigits = (98 - Mod97(string.Concat(uniqueNumber, digitalizedCode))).ToString("D2");

            string iban = string.Concat(countryCode, checkDigits, bankCode, uniqueNumber);

            return iban;
        }

        private int Mod97(string input)
        {
            string remainder = input;
            long block = 0;

            while (remainder.Length > 0)
            {
                int take = Math.Min(9, remainder.Length);
                block = long.Parse(block.ToString() + remainder.Substring(0, take));
                remainder = remainder.Substring(take);
                block %= 97;
            }

            return (int)block;
        }

        private string Digitalization(string letters)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in letters)
            {
                if (char.IsLetter(c))
                {
                    int value = char.ToUpper(c) - 'A' + 10;
                    stringBuilder.Append(value);
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
