using System.ComponentModel.DataAnnotations;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        //Id is the default name (by convention) of the primary key for the DB. If you want to change it, you have to specify [Key] above a different name, like so:
        /*
        Way above: using System.ComponentModel.DataAnnotations;

        [Key]
        public int TheId { get; set; }
        */
        public int Id { get; set; }
        public string? UserName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        public int GetAge() 
        {
            return DateOfBirth.CalculateAge();
        }
    }
}