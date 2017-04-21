using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IotLock.API.Hubs
{
    public class LightingHub : Hub
    {
        public void Toggle(string room)
        {
            Clients.Others.toggle(room);
        }
    }
}