using Microsoft.AspNetCore.Identity;

namespace TMS.Models.UserModels
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public List<Assignment> Assignments { get; set; }

        public bool IsAdmin { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
