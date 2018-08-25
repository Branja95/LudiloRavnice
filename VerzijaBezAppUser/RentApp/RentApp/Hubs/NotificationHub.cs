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
    [Authorize(Roles = "Admin")]
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
            hubContext.Clients.All.newUserAccountToApprove(count);
        }

        public static void NewRentVehicleServiceToApprove(long count)
        {
            hubContext.Clients.All.newRentVehicleServiceToApprove(count);
        }
    }
}