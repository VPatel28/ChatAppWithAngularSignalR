using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRWithAngular.SignalrAngular;
using SignalrAngular.Models;
using SignalrAngular.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace SignalrAngular.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatMessagesController : ControllerBase
    {
        private readonly Signalr _context;
        IHubContext<ChatHub> _hubContext;
        public ChatMessagesController(Signalr context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/ChatMessages
        [HttpGet]
        public IEnumerable<ChatMessage> GetChatMessage()
        {
            _hubContext.Clients.All.SendAsync("bindUsers", _context.ChatMessage);
            return _context.ChatMessage;
        }

        // GET: api/ChatMessages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatMessage = await _context.ChatMessage.FindAsync(id);

            if (chatMessage == null)
            {
                return NotFound();
            }

            return Ok(chatMessage);
        }

        // PUT: api/ChatMessages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatMessage([FromRoute] int id, [FromBody] ChatMessage chatMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chatMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(chatMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatMessageExists(id))
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

        [HttpPut("{id}")]
        public async Task<IActionResult> RemoveLoggedIn([FromRoute] int id, [FromBody] ChatMessage chatMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chatMessage.Id)
            {
                return BadRequest();
            }
            chatMessage.IsLoggedIn = 0;
            _context.Entry(chatMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("bindUsers", _context.ChatMessage);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatMessageExists(id))
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

        // POST: api/ChatMessages
        [HttpPost]
        public async Task<IActionResult> PostChatMessage([FromBody] ChatMessage chatMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ChatMessage res = _context.ChatMessage.Where(p => p.NickName.ToString().ToLower() == chatMessage.NickName.ToString().ToLower()).FirstOrDefault();

            if (res != null)
            {
                res.IsLoggedIn = 1;
                _context.Entry(res).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(res);
            }
            chatMessage.IsLoggedIn = 1;
            _context.ChatMessage.Add(chatMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatMessage", new { id = chatMessage.Id }, chatMessage);
        }

        // DELETE: api/ChatMessages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var chatMessage = await _context.ChatMessage.FindAsync(id);
            if (chatMessage == null)
            {
                return NotFound();
            }

            _context.ChatMessage.Remove(chatMessage);
            await _context.SaveChangesAsync();

            return Ok(chatMessage);
        }

        private bool ChatMessageExists(int id)
        {
            return _context.ChatMessage.Any(e => e.Id == id);
        }
    }
}