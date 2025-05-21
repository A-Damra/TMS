using System.ComponentModel.DataAnnotations;

namespace TMS.Models.PasswordModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
