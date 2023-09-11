using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        public DateOnly? DateOfBirth { get; set; } //Optional to make required work. This prevents default value, but we want it to be null so it can be required.
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 4)]
        public string Password { get; set; }
    }
}