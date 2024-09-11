using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
using VDS_Backend.Src.Utilities;
using WatsonWebserver.Core;

namespace VDS_Backend.Src.VDSServer
{
    /// <summary>
    /// Class that represents the interface between the client and the server.
    /// contains methods that the client and server agree upon, such as login, sign up and more.
    /// </summary>
    internal partial class ServerInterface
    {
        /// <summary>
        /// The live connection keys established by the client and server.
        /// maps connection key to the correlating email.
        /// </summary>
        private Dictionary<string, string> connections;
        /// <summary>
        /// database interface.
        /// </summary>
        private VDbInterface dbInterface;
        /// <summary>
        /// length of randomly generated connection key
        /// </summary>
        private const int CONNECTION_KEY_LENGTH = 16;

        /// <summary>
        /// Creates a new instance of the server interface: an interface
        /// between the client and the server, based on a given context.
        /// </summary>
        /// <param name="context">the context of the database</param>
        public ServerInterface(VDSContext context)
        {
            connections = new Dictionary<string, string>();
            dbInterface = new VDbInterface(context);
        }

        /// <summary>
        /// Adds a new user to the database from the given data sent to the server.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>Json resposnse, either same success/error response as login, or
        /// error and empty connectionKey if failed to sign up.</returns>
        public string SignUp(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var signupInput = SafeDeserializeObjectToJson<SignupInputPayload>(body);

            // status of operation
            // default is unknown payload if signupInput is null
            var status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;

            if (signupInput != null) // sanity check: signInput mustn't be unparsed
            {
                // result status
                status = dbInterface.SignUp(signupInput.FirstName, signupInput.LastName,
                    signupInput.Email, signupInput.Password, signupInput.Phone);
            }

            // login if successfully signed up
            if (status.Code == StatusCode.Success)
            { return Login(ctx); }

            // failed to signup response
            var failureResponse = new
            {
                operationStatus = SafeSerializeObject(status),
                connectionKey = ""
            };
            return SafeSerializeObject(failureResponse);
        }

        /// <summary>
        /// logs in a user into the system and returns a connection key if
        /// a valid user connection was established.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>Json response as a result. Either contains success and connection if succeeded,
        /// or otherwise some error and empty connection key if failed.</returns>
        public string Login(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var loginInput = SafeDeserializeObjectToJson<LoginInputPayload>(body);

            // Attempt to generate the connection key of the session
            // otherwise connection key stays empty
            string generatedConnectionKey = "";

            // status of operation
            // default is unknown payload if loginInput is null
            var status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;

            if (loginInput != null) // sanity check: input isn't null in order to attempt login
            {
                // result status
                status = dbInterface.Login(loginInput.Email, loginInput.Password);
                if (status.Code == StatusCode.Success)
                {
                    generatedConnectionKey = GenerateNewKey(loginInput.Email);
                }
            }

            // the response sent due to the request
            // connectionKey will stay empty if couldn't login
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
                connectionKey = generatedConnectionKey
            };

            return SafeSerializeObject(response);
        }

        /// <summary>
        /// closes the user connection, removing the connection key from memory.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>Success even if there is nothing to remove, for security reasons.</returns>
        public string Logout(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var logoutInput = SafeDeserializeObjectToJson<LogoutInputPayload>(body);
            
            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            if (logoutInput != null)
            {
                status = OperationStatus.SUCCESS;
                if (connections.ContainsKey(logoutInput.ConnectionKey))
                {
                    Console.WriteLine("\tSuccessfully logged out.");
                    connections.Remove(logoutInput.ConnectionKey);
                }
            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// Create a new recruitment post based on the given arguments.
        /// </summary>
        /// <param name="ctx"> the http context</param>
        /// <returns>success on success, otherwise unknown payload, invalid connection key or other errors</returns>
        public string CreatePost(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<CreatePostInputPayload>(body);

            // sanity check: input must be parsed correctly
            if (input is null) {
                var payloadErrResponse = new
                {
                    operationStatus = SafeSerializeObject(OperationStatus.UNKNOWN_PAYLOAD_ERROR),
                };
                return SafeSerializeObject(payloadErrResponse);
            }


            // user email of the connection key
            string? email = GetEmailFromKey(input.ConnectionKey);
            if (email is null) // sanity check: connection key must be exist
            {
                // the response sent due invalid connection key
                var errResponse = new
                {
                    operationStatus = SafeSerializeObject(OperationStatus.INVALID_CONNECTION_KEY_ERROR),
                };
                return SafeSerializeObject(errResponse);
            }

            // status result of creating post
            OperationStatus status = dbInterface.CreatePost(email, input.Title, input.Description, input.Address,
                input.VolunteerArea, input.JobType, input.InitialDate, input.LastDate, input.MaxVolunteers);
            // the response sent due to request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// gets the filtered posts from the db.
        /// may ignore filter options if they are empty or null.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string GetFilteredPosts(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<GetFilteredPostsInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            VDbHandler.PostInfo[] posts = [];
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    var opRes = dbInterface.GetFilteredPosts(input.VolunteerAreas, input.JobTypes,
                    input.InitialDate, input.EndDate, input.DateFilterType, email);
                    status = opRes.Item1;
                    posts = opRes.Item2;
                }
                

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
                posts = SafeSerializeObject(posts)
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// gets the user posts from the db.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string GetUserPosts(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<LogoutInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            VDbHandler.PostInfo[] posts = [];
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    var opRes = dbInterface.GetUserPosts(email);
                    status = opRes.Item1;
                    posts = opRes.Item2;
                }

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
                posts = SafeSerializeObject(posts)
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// edit a post in db.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string EditPost(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<EditPostInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    status = dbInterface.EditPost(email, input.Id, input.Title, input.Description,
                        input.Address, input.VolunteerArea, input.JobType, input.InitialDate, input.LastDate);
                }

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// deletes a post in db.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string DeletePost(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<ConnectionKeyPostInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                Console.WriteLine($"email: {email}, id: {input.Id}");
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    status = dbInterface.DeletePost(email, input.Id);
                }

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// allows a user to join a post if it's not full.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string JoinUserToPost(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<ConnectionKeyPostInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                Console.WriteLine($"email: {email}, id: {input.Id}");
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    status = dbInterface.JoinUserToPost(email, input.Id);
                }

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }

        /// <summary>
        /// allows a user to leave a post if they haven't left yet.
        /// </summary>
        /// <param name="ctx">the http context</param>
        /// <returns>responses</returns>
        public string LeavePost(HttpContextBase ctx)
        {
            // unload the payload
            string body = ctx.Request.DataAsString;
            var input = SafeDeserializeObjectToJson<ConnectionKeyPostInputPayload>(body);

            // result status
            // default status unknown payload
            OperationStatus status = OperationStatus.UNKNOWN_PAYLOAD_ERROR;
            if (input != null)
            {
                string? email = GetEmailFromKey(input.ConnectionKey);
                Console.WriteLine($"email: {email}, id: {input.Id}");
                if (email == null)
                {
                    status = OperationStatus.INVALID_CONNECTION_KEY_ERROR;
                }
                else
                {
                    status = dbInterface.LeavePost(email, input.Id);
                }

            }

            // the response sent due to the request
            var response = new
            {
                operationStatus = SafeSerializeObject(status),
            };
            return SafeSerializeObject(response);
        }



        /// <summary>
        /// Find and removes all posts in the database that have expired.
        /// That is, their last date is older than the current day.
        /// Implemented in ServerInterface too, to keep the layer architecture consistent.
        /// </summary>
        public void DeleteExpiredPosts()
        {
            dbInterface.DeleteExpiredPosts();
        }

        /// <summary>
        /// Clear all current connections of the server.
        /// </summary>
        public void ClearConnections()
        {
            connections.Clear();
        }

        /// <summary>
        /// Generates a connection key that is just now registered to the connections dict.
        /// </summary>
        /// <param name="email">email of the user to attach to the connection key</param>
        /// <returns>the connection key</returns>
        private string GenerateNewKey(string email)
        {
            string? connectionKey = null;
            // find key that is not registered
            do
            {
                connectionKey = Utils.GenerateRandomString(CONNECTION_KEY_LENGTH);
            }
            while (connections.ContainsKey(connectionKey));
            connections.Add(connectionKey, email);
            return connectionKey;
        }

        /// <summary>
        /// Returns the email of the user that matches the given key, or null if no user matches.
        /// </summary>
        /// <param name="connectionKey">connection key of logged in user.</param>
        /// <returns>the email of the user that matches the given key, or null if no user matches.</returns>
        private string? GetEmailFromKey(string connectionKey)
        {
            if (connections.ContainsKey(connectionKey))
            {
                return connections[connectionKey];
            }
            return null;
        }

        /// <summary>
        /// Use JsonConvert.DeserializeObject to convert json string to object T,
        /// if any exception occurs return null
        /// </summary>
        /// <typeparam name="T">Object to deserialize to</typeparam>
        /// <param name="json">json string to deserialize</param>
        /// <returns>Deserialized object, or null if failed.</returns>
        private T? SafeDeserializeObjectToJson<T>(string json) where T : class
        {
            T? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex) { return null; }
            return result;

        }

        /// <summary>
        /// Safely serialize an object into json string, by converting enums to string representation
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>json serialized object</returns>
        private string SafeSerializeObject(Object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() },
            };

            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
