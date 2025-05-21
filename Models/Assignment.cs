using System.ComponentModel.DataAnnotations.Schema;
using TMS.Models.UserModels;

namespace TMS.Models
{
    public class Assignment
    {
        public Guid AssignmentId { get; set; }

        [Column(TypeName = "Nvarchar(50)")]
        public string Name { get; set; }

        [Column(TypeName = "Nvarchar(50)")]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
