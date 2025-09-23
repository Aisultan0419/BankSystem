﻿using Application.Interfaces.Services;
using System.Text;
using Application.Interfaces.Repositories;
using Domain.Enums;
namespace Application.Services.AccountServices
{
    public class IbanService : IIBanService
    {
        private readonly IUserRepository _userRepository;
        public IbanService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }            
        public async Task<string> GetIban(AccountType accountType, Guid Id)
        {
            string unique_id = (Math.Abs(Id.GetHashCode()) % 10000).ToString("D4");
            string order_number = await _userRepository.GetOrderNumber() 
                ?? throw new InvalidOperationException("Failed to generate account order number");
            string account_type = ((int)accountType).ToString("D3");
            string unique_number = string.Concat(account_type, unique_id, order_number);

            const string country_code = "KZ";
            const string bank_code = "AISS";

            string letters = country_code + bank_code;

            string digitalized_code = Digitalization(letters);

            string checkDigits = (98 - Mod97(string.Concat(unique_number, digitalized_code))).ToString("D2");
            
            string iban = string.Concat(country_code, checkDigits, bank_code, unique_number);

            return iban;
        }
        public int Mod97(string input)
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
        public string Digitalization(string letters)
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
