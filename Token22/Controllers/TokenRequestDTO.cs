namespace Geriatria.Api.DTOs
{
    public class TokenRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Grant_Type { get; set; }
    }
}
