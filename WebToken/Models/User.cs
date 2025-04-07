using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace WebToken.Models
{
    public class User : IdentityUser  // IdentityUser'dan türedi
    {
        [Key]
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [Column("Username")]
        public string Username { get; set; }

        [Required]
        [Column("PasswordHash")]
        public string Password { get; set; }

        // Diğer özelleştirilmiş özellikler burada yer alabilir
    }
}