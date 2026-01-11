
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Accounts
{
     public record InterestRate
    {
        public required decimal Rate { get; init; }
        public required DateOnly EffectiveFrom { get; init; }
        public DateOnly? EffectiveTo { get; init; }
        public required SavingType Type { get; init; }
    }
}
