namespace Application.Users.Queries.ExternalLogin
{
    public class ExternalLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty; // payload.Subject
    }
}
