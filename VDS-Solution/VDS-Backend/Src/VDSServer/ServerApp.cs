using Newtonsoft.Json;
using VDS_Backend.Src.Models.VDS.Contexts;
using WatsonWebserver;
using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;

namespace VDS_Backend.Src.VDSServer
{
    /// <summary>
    /// The main class for running the server.
    /// the server listens for https request, retrieves data from the database and sends
    /// it back to sender.the server is also responsible for running Cron Jobs:
    /// that is jobs every interval of X time (like removing expired posts from the database)
    /// </summary>
    internal class ServerApp
    {
        /// <summary>
        /// The server instance
        /// </summary>
        private static Webserver server;
        /// <summary>
        /// the server side of the interface that sends/receives data from the database
        /// </summary>
        private static ServerInterface serverInterface;

        /// <summary>
        /// Run the server program. The server will listen to specific request and
        /// send info back to the sender.
        /// </summary>
        /// <param name="hostname">name of the host. e.g: "localhost"</param>
        /// <param name="port">the port of the server</param>
        public static async void Run(string hostname, int port)
        {
            Console.WriteLine($"Running the server...\nYou may connect at \"http://{hostname}:{port}\".\n");
            
            server = new HostBuilder(hostname, port, false, DefaultRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/signup", SignupRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.GET, "/login", LoginRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/logout", LogoutRoute)
                .Build();
            await using VDSContext db = new VDSContextSQLite();
            serverInterface = new ServerInterface(db);
            server.Start();
            OnStart();

            // Wait for user input and stop the server.
            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();
            StopServer();

        }

        // default route
        static async Task DefaultRoute(HttpContextBase ctx) =>
            await ctx.Response.Send("Hello from default route!");

        // route to sign up users
        static async Task SignupRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[SIGNUP] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.\n");
            string response = serverInterface.SignUp(ctx);
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to login as a user
        static async Task LoginRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[LOGIN] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.\n");
            string response = serverInterface.Login(ctx);
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to logout as a user
        static async Task LogoutRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[LOGOUT] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.Logout(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        /// <summary>
        /// execute code that should run after server creation
        /// </summary>
        private static void OnStart()
        {
            // TODO: run cron jobs
        }

        /// <summary>
        /// execute code that should run after stopping the server
        /// </summary>
        private static void OnStop()
        {
            // TODO: stop cron jobs
        }

        /// <summary>
        /// Safely stop the server, if it has been initialized with Run().
        /// </summary>
        private static void StopServer()
        {
            if (server != null)
            {
                server.Stop();
                server.Dispose();
                serverInterface.ClearConnections();
                OnStop();
                Console.WriteLine("Server has been safely stopped.");
            }
        }
    }
}
