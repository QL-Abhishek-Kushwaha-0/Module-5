namespace Blog_Application.DTO.ResponseDTOs
{
    public class PostResponseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
    }
}
