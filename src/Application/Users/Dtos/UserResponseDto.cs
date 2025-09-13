namespace Application.Users.Dtos
{
    public class UserResponseDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = [];
    }
}
