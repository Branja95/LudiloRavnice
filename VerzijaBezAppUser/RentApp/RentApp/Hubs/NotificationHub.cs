using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Timers;
using System.Data.Entity;
using RentApp.Persistance.UnitOfWork;
using System.Threading.Tasks;

namespace RentApp.Hubs
{

    [HubName("Notifications")]
    //[Authorize(Roles = "Admin")]
    public class NotificationHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        private static Timer t = new Timer();

        private IUnitOfWork unitOfWork { get; set; }

        public NotificationHub(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public static void NewUserAccountToApprove(long count)
        {
            //hubContext.Clients.Group("Admins").newUserAccountToApprove();
        }

        public static void NewRentVehicleServiceToApprove(long count)
        {
            //hubContext.Clients.Group("Admins").newRentVehicleServiceToApprove(count);
        }

        public override Task OnConnected()
        {

            Groups.Add(Context.ConnectionId, "Admins");

            //if (Context.User.IsInRole("Admin"))
            //{
            //    Groups.Add(Context.ConnectionId, "Admins");
            //}

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            Groups.Add(Context.ConnectionId, "Admins");

            //if (Context.User.IsInRole("Admin"))
            //{
            //    Groups.Remove(Context.ConnectionId, "Admins");
            //}

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Test sat
        /// </summary>

        public void GetTime()
        {
            Clients.All.setRealTime(DateTime.Now.ToString("h:mm:ss tt"));
        }

        public void TimeServerUpdates()
        {
            t.Interval = 1000;
            t.Start();
            t.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            GetTime();
        }
    }
}