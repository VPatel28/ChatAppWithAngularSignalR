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
    public class LogModel : PageModel
    {
        private readonly SignalrAngular.Models.Signalr _context;

        public LogModel(SignalrAngular.Models.Signalr context)
        {
            _context = context;
        }

        public IList<LogData> LogData { get;set; }

        public async Task OnGetAsync()
        {
            LogData = await _context.LogData.ToListAsync();
        }
    }
}
