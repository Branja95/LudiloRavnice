using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RentApp.Hubs
{
    [HubName("Notifications")]
    public class NotificationsHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void GetRealTime()
        {
            Clients.All.setRealTime(DateTime.Now.ToString("h:mm:ss tt"));
        }
    }
}