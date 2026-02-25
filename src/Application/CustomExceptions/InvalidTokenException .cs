
namespace Application.CustomExceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base("User ID could not be obtained from token.")
        {
            
        }
    }
}
