using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using AMS.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TableDependency.SqlClient;

namespace AMS.SignalR
{
    [HubName("myHub")]
    public class MyHub : Hub
    {
        // Is set via the constructor on each creation
        //private Broadcaster _broadcaster;
        private Entities db = new Entities();


        //public MyHub()
        //    : this(Broadcaster.Instance)
        //{

        //}

        //public MyHub(Broadcaster broadcaster)
        //{
        //    _broadcaster = broadcaster;

        //}

        public void Send(string name, string message)
        {
            Dictionary<string,string> IDtoName = db.Employees.ToDictionary(n => n.EmployeeID, n => n.EmployeeName);
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(IDtoName[name], message);
        }

    }


    /// <summary>
    /// 數據廣播器
    /// </summary>
    //public class Broadcaster
    //{
    //    private readonly static Lazy<Broadcaster> _instance =
    //        new Lazy<Broadcaster>(() => new Broadcaster());

    //    private readonly IHubContext _hubContext;

    //    private Timer _broadcastLoop;

    //    public Broadcaster()
    //    {
    //        // 獲取所有連接的句柄，方便後面進行消息廣播
    //        _hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
    //        // Start the broadcast loop
    //        _broadcastLoop = new Timer(
    //            BroadcastShape,
    //            null,
    //            1000,
    //            int.MaxValue);
    //        //用SqlDependency Call BroadcastShape

    //    }



    //    private Random random = new Random();


    //    private void BroadcastShape(object state)
    //    {
    //        // 定期執行的方法
    //        _hubContext.Clients.All.sendTest1(random.Next(1000).ToString());
    //    }               
    //    public static Broadcaster Instance
    //    {
    //        get
    //        {
    //            return _instance.Value;
    //        }
    //    }
    ////}
}