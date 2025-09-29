using Application.Interfaces.Auth;
namespace Infrastructure
{
    public class Hasher : IHasher
    {
        public string Generate(string input) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(input);
        public bool Verify(string input, string hashedInput) =>
            BCrypt.Net.BCrypt.EnhancedVerify(input, hashedInput);
    }
}
