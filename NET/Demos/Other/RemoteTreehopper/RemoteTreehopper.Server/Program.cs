using System;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.Owin.Hosting;
using System.Threading;
using Treehopper;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Remote.Server
{
    class Program
    {
        public static Dictionary<string, TreehopperUsb> Boards { get; set; } = new Dictionary<string, TreehopperUsb>();
        static void Main(string[] args)
        {
            string url = "http://+:8080";

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            var serializer = JsonSerializer.Create(serializerSettings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.WriteLine("Press enter to exit");

                var clients = GlobalHost.ConnectionManager.GetHubContext<RemoteTreehopperHub>().Clients.All;

                ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
                // add existing boards
                foreach (var board in ConnectionService.Instance.Boards)
                {
                    Boards.Add(board.SerialNumber, board);
                    clients.BoardAdded(board.SerialNumber);
                }


                Console.ReadLine();
            }
        }

        private static void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var clients = GlobalHost.ConnectionManager.GetHubContext<RemoteTreehopperHub>().Clients.All;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var item in e.NewItems)
                {
                    var board = (TreehopperUsb)item;
                    Console.WriteLine("Adding board: " + board);
                    Boards.Add(board.SerialNumber, board);
                    clients.BoardAdded(board.SerialNumber);
                }
                    
            } else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var board = (TreehopperUsb)item;
                    Console.WriteLine("Removing board: " + board);
                    Boards.Remove(board.SerialNumber);
                    clients.BoardRemoved(board.SerialNumber);
                }
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.UseCors(CorsOptions.AllowAll);
            //app.MapSignalR(hubConfiguration);
            app.MapSignalR();
        }
    }
}
