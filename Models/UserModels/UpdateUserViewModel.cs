using System.ComponentModel.DataAnnotations;

namespace TMS.Models.UserModels
{
    public class UpdateUserViewModel
    {
        public string UserId { get; set; }


        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public int DepartmentId { get; set; }

        public bool IsAdmin { get; set; }

    }
}
