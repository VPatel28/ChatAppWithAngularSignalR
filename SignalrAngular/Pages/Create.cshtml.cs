using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRWithAngular.SignalrAngular;
using SignalrAngular.Models;

namespace SignalrAngular.Pages
{
    public class CreateModel : PageModel
    {
        private readonly SignalrAngular.Models.Signalr _context;

        public CreateModel(SignalrAngular.Models.Signalr context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public LogData LogData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LogData.Add(LogData);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}