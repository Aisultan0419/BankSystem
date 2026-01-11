using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Services.Pans;
using Domain.Enums;
using Domain.Models;
namespace Infrastructure.PanServices
{
    public class Pan_generation : IPanService
    {
        public string CreatePan(Client client)
        {
            const string MII = "6";
            const string issuerBin = "10182";

            const string BIN = MII + issuerBin;

            byte[] bytes = Encoding.UTF8.GetBytes(client.Id.ToString());

            using SHA256 sha256 = SHA256.Create();

            bytes = sha256.ComputeHash(bytes);

            uint value = BitConverter.ToUInt32(bytes, 0);

            string unique_number = value.ToString("D6");

            string card_type = ((int)CardType.Debit).ToString();

            string card_number = string.Concat(card_type, unique_number);

            string control_number = ControlNumber(BIN, card_number);

            string pan = string.Concat(BIN, card_number, control_number);

            return pan;
        }
        public string ControlNumber(string BIN, string card_number)
        {
            string pan_part = BIN + card_number;
            bool double_digit = true;
            int sum = 0;
            for (int i = pan_part.Length - 1; i > 1; i--)
            {
                int digit = int.Parse(pan_part[i].ToString());
                if (double_digit)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }
                sum += digit;
                double_digit = !double_digit;
            }
            return ((10 - sum % 10) % 10).ToString();
        }
    }
}
