using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SignalRWithAngular.SignalrAngular;

namespace SignalrAngular.Models
{
    public class Signalr : DbContext
    {
        public Signalr (DbContextOptions<Signalr> options)
            : base(options)
        {
        }

        public DbSet<SignalRWithAngular.SignalrAngular.ChatMessage> ChatMessage { get; set; }

        public DbSet<SignalRWithAngular.SignalrAngular.ChatHistory> ChatHistory { get; set; }
        public DbSet<SignalRWithAngular.SignalrAngular.LogData> LogData { get; set; }
    }
}
