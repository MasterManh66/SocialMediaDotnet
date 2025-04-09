using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Entity
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string RoleName { get; set; }
    }
}
