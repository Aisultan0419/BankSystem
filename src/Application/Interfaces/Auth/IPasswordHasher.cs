namespace Application.Interfaces.Auth
{
    public interface IHasher
    {
        string Generate(string input);
        bool Verify(string input, string hashedInput);
    }
}
