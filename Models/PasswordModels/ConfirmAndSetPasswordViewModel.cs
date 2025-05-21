using System.ComponentModel.DataAnnotations;
namespace TMS.Models.PasswordModels;
public class ConfirmAndSetPasswordViewModel
{
    public string UserId { get; set; }
    public string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage ="Password do not match.")]
    public string ConfirmPassword { get; set; }
}
