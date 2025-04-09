using System.ComponentModel.DataAnnotations;
using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public string Job { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public GenderEnum Gender { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
