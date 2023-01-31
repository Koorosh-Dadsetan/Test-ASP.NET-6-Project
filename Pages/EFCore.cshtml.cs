using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test_Project.Models;

namespace Test_Project.Pages
{
    public class EFCoreModel : PageModel
    {
        private readonly DBCtx _context;

        public EFCoreModel(DBCtx context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Employees != null)
            {
                Employee = await _context.Employees.ToListAsync();
            }
        }

        


    }
}
