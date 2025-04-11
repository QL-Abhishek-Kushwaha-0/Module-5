using Blog_Application.Models.Entities;

namespace Blog_Application.DTO
{
    public class WithAuthor:Category
    {
        public List<User>? Author { get; set; }    
    }
}
