using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SignalrAngular.Models;
using SignalRWithAngular.SignalrAngular;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalrAngular.Hubs
{
    public class ChatHub : Hub
    {
        Signalr appContext;
        public static StringBuilder stringBuilder= new StringBuilder();
        public void SaveLog(string Log)
        {
            stringBuilder.AppendLine(Log);
        }

        public ChatHub()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
         .AddJsonFile("appsettings.json")
         .Build();

            var options = new DbContextOptionsBuilder<Signalr>()
                   .UseSqlServer(configuration.GetConnectionString("Signalr"))
                   .Options;

            appContext = new Signalr(options);
        }

        public async Task UserConnected(int id)
        {
            await this.Clients.All.SendAsync("ConnectedUser", id);
            SaveLog("User Connected with ID =>" + id);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            LogData logData = new LogData();
            logData.LogD = stringBuilder.ToString();
            stringBuilder.Clear();
            appContext.LogData.Add(logData);
            appContext.SaveChangesAsync();
      
            return base.OnDisconnectedAsync(exception);
        }

        public async Task FetchUsers()
        {
            await this.Clients.All.SendAsync("bindUsers", appContext.ChatMessage.Where(p => p.IsLoggedIn == 1));
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();

        }

        public async Task SendMessage(string user, string message)
        {
            string Room = "";
            if (Context.Items.ContainsKey("Room"))
            {
                Room = (String)Context.Items["Room"];
            }
            await Clients.Group(Room).SendAsync(user, message);
        }

        public async Task CreateGroup(int currentuserid, int respectiveuserid)
        {
            string groupname = "";

            int i = currentuserid;
            int j = respectiveuserid;

            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
            SaveLog("Group Created for Userid " + currentuserid + " with connection id" + Context.ConnectionId + " for respective user " + respectiveuserid + " Group Name " + groupname);
            await Clients.Group(groupname).SendAsync("GroupCreated", currentuserid, respectiveuserid, groupname);
        }

        public async Task SaveNonActiveGroup(int respectiveuserid, int currentuserid)
        {
            string groupname = "";

            int i = currentuserid;
            int j = respectiveuserid;

            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }

            SaveLog("NonActiveGroup Created for Userid " + currentuserid + " with connection id" + Context.ConnectionId + " for respective user " + respectiveuserid + " Group Name " + groupname);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
        }

        public async Task SaveHistory(int curruserid, int respuserid, int resuseridcurrnt, string msghtm)
        {
            if (respuserid == 0)
                respuserid = resuseridcurrnt;

            string groupname = "";
            int i = curruserid;
            int j = respuserid;

            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }

            ChatHistory chatHistory;

            // ChatMessage chatcurrent = await appContext.ChatMessage.FindAsync(curruserid);
            ///  ChatMessage chatrespec = await appContext.ChatMessage.FindAsync(respuserid);

            chatHistory = await appContext.ChatHistory.Where(p => p.UserId == curruserid && p.GrpName == groupname).FirstOrDefaultAsync();
            if (chatHistory == null)
            {
                chatHistory = new ChatHistory();
                chatHistory.UserId = curruserid;
                if (msghtm != "")
                {
                    chatHistory.ChatData = msghtm;
                }
                chatHistory.GrpName = groupname;
                appContext.ChatHistory.Add(chatHistory);
            }
            else
            {
                if (msghtm != "")
                {
                    chatHistory.ChatData = msghtm;
                }
                appContext.ChatHistory.Attach(chatHistory);
                appContext.Entry(chatHistory).State = EntityState.Modified;
            }
            await appContext.SaveChangesAsync();

            SaveLog("SaveHistory for Userid " + curruserid + " with connection id" + Context.ConnectionId + " for respective user " + resuseridcurrnt + " Group Name " + groupname);


            await Clients.All.SendAsync("HistorySaved", curruserid, resuseridcurrnt);
        }

        public async Task FetchHistory(int currentuserid, int respectiveUserid)
        {
            string groupname = "";
            int i = currentuserid;
            int j = respectiveUserid;

            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

            ChatHistory chatHistory = await appContext.ChatHistory.Where(p => p.UserId == currentuserid && p.GrpName == groupname).FirstOrDefaultAsync();
            string history;
            if (chatHistory == null)
            {
                chatHistory = new ChatHistory();
                chatHistory.UserId = currentuserid;
                chatHistory.ChatData = "";
                chatHistory.GrpName = groupname;
                appContext.ChatHistory.Add(chatHistory);
                await appContext.SaveChangesAsync();
            }
            history = chatHistory.ChatData;

            SaveLog("FetchHistory for Userid " + currentuserid + " with connection id" + Context.ConnectionId + " for respective user " + respectiveUserid + " Group Name " + groupname + " History " + history);

            await Clients.Client(Context.ConnectionId).SendAsync("OpenChatWi", appContext.ChatMessage.Where(p => p.IsLoggedIn == 1), respectiveUserid, history);
        }

        public async Task UpdateHistory(int currentuserid, int respectiveuserid, int respectivecurrntid, string msghtm, string add)
        {

            if (respectiveuserid == 0)
                respectiveuserid = respectivecurrntid;

            string groupname = "";

            ChatHistory chatHistory;

            int i = currentuserid;
            int j = respectiveuserid;

            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }

            chatHistory = await appContext.ChatHistory.Where(p => p.UserId == currentuserid && p.GrpName == groupname).FirstOrDefaultAsync();



            if (chatHistory == null)
            {
                chatHistory = new ChatHistory();
                chatHistory.UserId = currentuserid;
                if (msghtm != "")
                {
                    if (add == "1")
                        chatHistory.ChatData = chatHistory.ChatData + msghtm;
                    else
                        chatHistory.ChatData = msghtm;
                }

                chatHistory.GrpName = groupname;
                appContext.ChatHistory.Add(chatHistory);
            }
            else
            {
                if (msghtm != "")
                {
                    if (add == "1")
                        chatHistory.ChatData = chatHistory.ChatData + msghtm;
                    else
                        chatHistory.ChatData = msghtm;
                }

                appContext.ChatHistory.Attach(chatHistory);
                appContext.Entry(chatHistory).State = EntityState.Modified;
            }

            SaveLog("UpdateHistory for Userid " + currentuserid + " with connection id" + Context.ConnectionId + " for respective user " + respectiveuserid + " Group Name " + groupname + " History " + msghtm);

            await appContext.SaveChangesAsync();

        }


        public async Task PassMessage(int currentuserid, int respectiveuserid, string message, string msghtml, string currentusername, string respectiveusername)
        {
            string groupname = "";
            int i = currentuserid;
            int j = respectiveuserid;
            if (i < j)
            {
                groupname = i + "-" + j;
            }
            else
            {
                groupname = j + "-" + i;
            }

            SaveLog("Passing Message for Userid " + currentuserid + " with connection id" + Context.ConnectionId + " for respective user " + respectiveuserid + " Group Name " + groupname + " msg " + message);

           
            await Clients.Groups(groupname).SendAsync("ShowMessage", currentuserid, respectiveuserid, message, groupname, respectiveusername, currentusername);
        }
    }


}
