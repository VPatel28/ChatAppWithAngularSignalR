using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SignalRWithAngular.SignalrAngular;
using SignalrAngular.Models;

namespace SignalrAngular.Pages
{
    public class EditModel : PageModel
    {
        private readonly SignalrAngular.Models.Signalr _context;

        public EditModel(SignalrAngular.Models.Signalr context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LogData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogDataExists(LogData.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LogDataExists(int id)
        {
            return _context.LogData.Any(e => e.id == id);
        }
    }
}
