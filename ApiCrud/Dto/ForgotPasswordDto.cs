using System.ComponentModel.DataAnnotations;

namespace ApiCrud.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
