using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;

namespace TMS.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly TMSDbContext tmsDbContext;
        public DepartmentController(TMSDbContext tmsDbContext)
        {
            this.tmsDbContext = tmsDbContext;   
        }

        public async Task<IActionResult> DepartmentList()
        {
            var model = await tmsDbContext.departments.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                await tmsDbContext.departments.AddAsync(department);
                await tmsDbContext.SaveChangesAsync();
                return RedirectToAction("DepartmentList");
            }
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            var department = await tmsDbContext.departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                tmsDbContext.departments.Update(department);
                await tmsDbContext.SaveChangesAsync();
                return RedirectToAction("DepartmentList");
            }
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await tmsDbContext.departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await tmsDbContext.departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            tmsDbContext.departments.Remove(department);
            await tmsDbContext.SaveChangesAsync();
            return RedirectToAction("DepartmentList");
        }
    }
}
