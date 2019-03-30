using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangman.WebUI.Controllers
{
    public class SignalRCounter : Hub
    {
        public Task IncrementCounter()
        {

            List<String> ConnectionIDToIgnore = new List<String>();
            ConnectionIDToIgnore.Add(Context.ConnectionId);
            return Clients.AllExcept(ConnectionIDToIgnore).SendAsync("IncrementCounter");
        }

        public Task DecrementCounter()
        {
            List<String> ConnectionIDToIgnore = new List<String>();
            ConnectionIDToIgnore.Add(Context.ConnectionId);
            return Clients.AllExcept(ConnectionIDToIgnore).SendAsync("DecrementCounter");
        }
    }
}
