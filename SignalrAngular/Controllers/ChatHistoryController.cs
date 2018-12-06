using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRWithAngular.SignalrAngular;
using SignalrAngular.Models;

namespace SignalrAngular.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatHistoryController : ControllerBase
    {
        private readonly Signalr _context;

        public ChatHistoryController(Signalr context)
        {
            _context = context;
        }

        // GET: api/ChatHistory
        [HttpGet]
        public IEnumerable<ChatHistory> GetChatHistory()
        {
            return _context.ChatHistory;
        }

        // GET: api/ChatHistory/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatHistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatHistory = await _context.ChatHistory.FindAsync(id);

            if (chatHistory == null)
            {
                return NotFound();
            }

            return Ok(chatHistory);
        }

        // PUT: api/ChatHistory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatHistory([FromRoute] int id, [FromBody] ChatHistory chatHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chatHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(chatHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatHistoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChatHistory
        [HttpPost]
        public async Task<IActionResult> PostChatHistory([FromBody] ChatHistory chatHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ChatHistory.Add(chatHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatHistory", new { id = chatHistory.Id }, chatHistory);
        }

        // DELETE: api/ChatHistory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatHistory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatHistory = await _context.ChatHistory.FindAsync(id);
            if (chatHistory == null)
            {
                return NotFound();
            }

            _context.ChatHistory.Remove(chatHistory);
            await _context.SaveChangesAsync();

            return Ok(chatHistory);
        }

        private bool ChatHistoryExists(int id)
        {
            return _context.ChatHistory.Any(e => e.Id == id);
        }
    }
}