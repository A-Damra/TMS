using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS.Models.UserModels;

namespace TMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Column(TypeName = "Nvarchar(50)")]
        public string Name { get; set; }


        public List<User>? Users { get; set; }
    }
}
