using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TMS.ViewModels
{
    public class AddAssignmentViewModel
    {
        public Guid ProjectId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string? UserId { get; set; }

        public List<SelectListItem> Users { get; set; } = new();
    }
}
