
using System.ComponentModel.DataAnnotations;

namespace Blog_Application.Models.Entities
{
    public class Subscription
    {
        [Key]
        public Guid AuhtorId { get; set; }
        public Guid ViewerId { get; set; }
    }
}
