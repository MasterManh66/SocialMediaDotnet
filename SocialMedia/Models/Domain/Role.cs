using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string? RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
