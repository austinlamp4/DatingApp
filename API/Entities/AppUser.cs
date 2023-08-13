using System.ComponentModel.DataAnnotations;

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
    }
}