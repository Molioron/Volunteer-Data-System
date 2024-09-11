using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using System.Reflection;
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
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/joinpost", JoinUserToPostRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/leavepost", LeavePostRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/viewvolunteers", ViewVolunteersRoute)
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

        // Route to sign up users
        static async Task SignupRoute(HttpContextBase ctx)
        {
            await HandleRoute("SIGNUP", ctx, serverInterface.SignUp);
        }

        // Route to login as a user
        static async Task LoginRoute(HttpContextBase ctx)
        {
            await HandleRoute("LOGIN", ctx, serverInterface.Login);
        }

        // Route to logout as a user
        static async Task LogoutRoute(HttpContextBase ctx)
        {
            await HandleRoute("LOGOUT", ctx, serverInterface.Logout);
        }

        // Route to create a new post
        static async Task CreatePostRoute(HttpContextBase ctx)
        {
            await HandleRoute("CREATE POST", ctx, serverInterface.CreatePost);
        }

        // Route to getting filtered posts
        static async Task GetFilteredPostsRoute(HttpContextBase ctx)
        {
            await HandleRoute("GET FILTERED POSTS", ctx, serverInterface.GetFilteredPosts);
        }

        // Route to getting user posts
        static async Task GetUserPostsRoute(HttpContextBase ctx)
        {
            await HandleRoute("GET USER POSTS", ctx, serverInterface.GetUserPosts);
        }

        // Route to edit a post
        static async Task EditPostRoute(HttpContextBase ctx)
        {
            await HandleRoute("EDIT POST", ctx, serverInterface.EditPost);
        }

        // Route to delete a post
        static async Task DeletePostRoute(HttpContextBase ctx)
        {
            await HandleRoute("DELETE POST", ctx, serverInterface.DeletePost);
        }

        // Route to join user to post request
        static async Task JoinUserToPostRoute(HttpContextBase ctx)
        {
            await HandleRoute("JOIN USER TO POST", ctx, serverInterface.JoinUserToPost);
        }

        // Route to leave post request
        static async Task LeavePostRoute(HttpContextBase ctx)
        {
            await HandleRoute("LEAVE POST", ctx, serverInterface.LeavePost);
        }

        // Route to view volunteers of post request
        static async Task ViewVolunteersRoute(HttpContextBase ctx)
        {
            await HandleRoute("VIEW VOLUNTEERS", ctx, serverInterface.ViewVolunteers);
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

        /// <summary>
        /// Describes what a route has to do to be handled.
        /// Calls some common methods that all route call, and then also run route specific method.
        /// </summary>
        /// <param name="name">name of the route</param>
        /// <param name="ctx">the http context of the server</param>
        /// <param name="routeMethod">the method the specific route calls to handle data passed through it</param>
        private static async Task HandleRoute(string name, HttpContextBase ctx, Func<HttpContextBase, string> routeMethod)
        {
            Console.WriteLine($"[{name}] {ctx.Request.Source.IpAddress}:\n\t{ctx.Request.DataAsString}.");
            string response = routeMethod(ctx);
            Console.WriteLine();
            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            
            await ctx.Response.Send(response); // caller needs to await HandleRoute
        }
    }
}
