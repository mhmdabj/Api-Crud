using System.ComponentModel.DataAnnotations;

namespace ApiCrud.Dto
{
    public class ResetPasswordDto
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Repassword { get; set; }
    }
}
