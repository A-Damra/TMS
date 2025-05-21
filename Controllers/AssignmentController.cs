using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;

namespace TMS.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly TMSDbContext _context;

        public AssignmentController(TMSDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AssignmentList()
        {
            var assignments = await _context.assignments.ToListAsync();
            return View(assignments);
        }


        [HttpGet]
        public IActionResult CreateAssignment()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.AssignmentId = Guid.NewGuid();
                await _context.assignments.AddAsync(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AssignmentList));
            }
            return View(assignment);
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignment(Guid id)
        {
            var assignment = await _context.assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.assignments.Update(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AssignmentList));
            }
            return View(assignment);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            var assignment = await _context.assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AssignmentList));
        }
    }
}
