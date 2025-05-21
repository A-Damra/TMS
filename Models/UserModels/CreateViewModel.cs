using System.ComponentModel.DataAnnotations;

namespace TMS.Models.UserModels
{
    public class CreateViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}
