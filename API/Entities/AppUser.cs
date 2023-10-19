using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}