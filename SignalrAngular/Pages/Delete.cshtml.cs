using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRWithAngular.SignalrAngular;
using SignalrAngular.Models;

namespace SignalrAngular.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly SignalrAngular.Models.Signalr _context;

        public DeleteModel(SignalrAngular.Models.Signalr context)
        {
            _context = context;
        }

        [BindProperty]
        public LogData LogData { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LogData = await _context.LogData.FirstOrDefaultAsync(m => m.id == id);

            if (LogData == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LogData = await _context.LogData.FindAsync(id);

            if (LogData != null)
            {
                _context.LogData.Remove(LogData);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
