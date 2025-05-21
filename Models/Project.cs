using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS.Models.UserModels;

namespace TMS.Models
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }

        [Column(TypeName = "Nvarchar(50)")]
        public string ProjectName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();


        public List<User>? Users { get; set; }

    }
}
