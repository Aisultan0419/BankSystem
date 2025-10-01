namespace Application.DTO.ClientDTO
{
    public class RegistrationStatusDTO
    {
        public string KycStatus { get; set; } = "NotStarted";
        public string VerificationStatus { get; set; } = "NotStarted";
        public string Message { get; set; } = "";
    }
}
