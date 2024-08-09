using Newtonsoft.Json;
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
        /// Run the server program. The server will listen to specific request and
        /// send info back to the sender.
        /// </summary>
        /// <param name="hostname">name of the host. e.g: "localhost"</param>
        /// <param name="port">the port of the server</param>
        public static void Run(string hostname, int port)
        {
            Console.WriteLine($"Running the server...\nYou may connect at \"http://{hostname}:{port}\".\n");
            
            server = new HostBuilder(hostname, port, false, DefaultRoute)
                .MapParameterRoute(WatsonWebserver.Core.HttpMethod.GET, "/greet", GreetRoute)
                .MapStaticRoute(WatsonWebserver.Core.HttpMethod.POST, "/goodbye", GoodbyeRoute)
                .Build();
            server.Start();
            OnStart();

            // Wait for user input and stop the server.
            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();
            StopServer();

        }

        static async Task DefaultRoute(HttpContextBase ctx) =>
            await ctx.Response.Send("Hello from default route!");

        /*
         Dummy request
         */
        static async Task GreetRoute(HttpContextBase ctx)
        {
            if (ctx.Request.Query.Elements.AllKeys.Contains("name"))
            {
                string? name = ctx.Request.Query.Elements["name"];
                var response = new
                {
                    Greeting = $"Hello, {name}!",
                    Timestamp = DateTime.UtcNow,
                    Status = "Success"
                };
                string jsonResponse = JsonConvert.SerializeObject(response);
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.Send(jsonResponse);
            }
            else
            {
                var errorResponse = new
                {
                    Error = "Bad Request",
                    Message = "'name' parameter is missing"
                };
                string jsonErrorResponse = JsonConvert.SerializeObject(errorResponse);
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.Send(jsonErrorResponse);
            }
        }
        static async Task GoodbyeRoute(HttpContextBase ctx)
        {
            // Read the request body
            string requestBody = ctx.Request.DataAsString;

            // Deserialize the JSON to get the "name" parameter
            var requestData = JsonConvert.DeserializeObject<RequestData>(requestBody);

            // Create the response message
            var responseMessage = new { message = $"Goodbye, {requestData.Name}!" };

            // Serialize the response message to JSON
            string jsonResponse = JsonConvert.SerializeObject(responseMessage);

            // Send the JSON response
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(jsonResponse);
        }
        private class RequestData
        {
            public string Name { get; set; }
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
                OnStop();
                Console.WriteLine("Server has been safely stopped.");
            }
        }
    }
}
