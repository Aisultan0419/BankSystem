using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
   public readonly struct Money
    {
        public decimal Amount { get; }
        public string Currency { get; }
        public Money(decimal amount, string currency = "KZT")
        {
            Amount = amount;
            Currency = currency;
        }
        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add Money with different currencies.");
            return new Money(a.Amount + b.Amount, a.Currency);
        }
    }
}
