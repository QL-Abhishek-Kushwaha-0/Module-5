namespace Blog_Application.DTO.ResponseDTOs
{
    public class LoginResponseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Username { get; set; }
        public string? AuthToken { get; set; }    
    }
}
