using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Utilities;
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
        /// The web server instance 
        /// </summary>
        private static Webserver server;
        /// <summary>
        /// the server side of the interface that sends/receives data from the database
        /// </summary>
        private static ServerInterface serverInterface;

        private static BackgroundJobServer hangfireServer;


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
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/login", LoginRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/logout", LogoutRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/createpost", CreatePostRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/getfilteredposts", GetFilteredPostsRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/getuserposts", GetUserPostsRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/editpost", EditPostRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/deletepost", DeletePostRoute)
                .Build();
            await using VDSContext db = new VDSContextSQLite();
            serverInterface = new ServerInterface(db);
            server.Start();

            // Configure Hangfire with memory storage (or other storage if needed)
            GlobalConfiguration.Configuration.UseMemoryStorage();
            // start the hangfire server
            hangfireServer = new BackgroundJobServer();

            // hangfire jobs and reocurring job manager initialized in OnStart()
            try { OnStart(); }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            // print sceduled jobs
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();

                Console.WriteLine("Scheduled Recurring Jobs:");
                foreach (var job in recurringJobs)
                {
                    Console.WriteLine($"Job ID: {job.Id}");
                    Console.WriteLine($"Cron Expression: {job.Cron}");
                    Console.WriteLine($"Next Execution Time: {job.NextExecution} (utc time)");
                    Console.WriteLine();
                }
            }

            // Wait for user input and stop the server.
            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();
            StopServer();

        }


        /// <summary>
        /// execute code that should run after web server creation
        /// </summary>
        private static void OnStart()
        {
            // time from local time to utc.
            DateTime utcTime = Utils.ConvertToUtc(8, 0);

            int minute = utcTime.Minute;

            int hour = utcTime.Hour;


            // Schedule the DeleteExpiredPostsTask to run daily at a specific time
            // The time is hour:minute
            RecurringJob.AddOrUpdate(
                "DeleteExpiredPostsTask",
                () => DeleteExpiredPostsTask(),
                $"{minute} {hour} * * *"
            );
            Console.WriteLine("Hangfire job scheduled: DeleteExpiredPostsTask");
        }

        /// <summary>
        /// execute code that should run after stopping the web server
        /// </summary>
        private static void OnStop()
        {
            // Remove the task to delete expired posts
            RecurringJob.RemoveIfExists("DeleteExpiredPostsTask");
            Console.WriteLine("Hangfire job unscheduled: DeleteExpiredPostsTask");
        }

        /// <summary>
        /// Deletes all posts that have expired (their last Date is older than the current day.)
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteExpiredPostsTask()
        {
            Console.WriteLine($"Deleting expired posts task running at: {DateTime.Now} (local time)");
            serverInterface.DeleteExpiredPosts();
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

        // route to create a new post
        static async Task CreatePostRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[CREATE POST] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.CreatePost(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to getting filtered posts
        static async Task GetFilteredPostsRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[GET FILTERED POSTS] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.GetFilteredPosts(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to getting user posts
        static async Task GetUserPostsRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[GET USER POSTS] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.GetUserPosts(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to edit a post
        static async Task EditPostRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[EDIT POST] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.EditPost(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        // route to delete post request
        static async Task DeletePostRoute(HttpContextBase ctx)
        {
            Console.WriteLine($"[DELETE POST] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = serverInterface.DeletePost(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(response);
        }

        /// <summary>
        /// Safely stop the server, if it has been initialized with Run().
        /// </summary>
        private static void StopServer()
        {
            if (server != null)
            {
                server.Stop(); // stop webserver
                server.Dispose();
                hangfireServer.Dispose(); // stop the hangfire server
                serverInterface.ClearConnections();
                OnStop();
                Console.WriteLine("Server has been safely stopped.");
            }
        }
    }
}
