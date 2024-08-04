using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Utilities;

namespace VDS_Backend.Src.VDSServer
{
    /// <summary>
    /// Class that represents the interface between the client and the server.
    /// contains methods that the client and server agree upon, such as login, sign up and more.
    /// </summary>
    internal class ServerInterface
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
        /// Adds a new user to the database from the given data.
        /// </summary>
        /// <param name="firstName">user's first name</param>
        /// <param name="lastName">user's last name</param>
        /// <param name="email">user's email</param>
        /// <param name="password">password</param>
        /// <param name="phoneNumber">user's phone number</param>
        /// <returns>Success when signed up successfully, otherwise AlreadyExistsError</returns>
        public void SignUp(string firstName, string lastName, string email, string password, string phoneNumber)
        {
            var status = dbInterface.SignUp(firstName, lastName, email, password, phoneNumber);
            Console.WriteLine($"sign up status: {status}");
            if (status == OperationStatus.Success)
            { Login(email, password); }
        }

        /// <summary>
        /// logs in a user into the system and returns a connection key if
        /// a valid user connection was established.
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="password">password of the user</param>
        /// <returns>Sucess and a connection key if logged in successfully, otherwise CredentialsError.</returns>
        public void Login(string email, string password)
        {
            var status = dbInterface.Login(email, password);
            string? connectionKey = null;
            if (status == OperationStatus.Success)
            {
                connectionKey = GenerateNewKey(email);
            }
            Console.WriteLine($"login to user \"{email}\" status: {status}, key: {connectionKey}");
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
    }
}
