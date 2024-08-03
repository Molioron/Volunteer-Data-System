using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.Tables;
using VDS_Backend.Src.Utilities;

namespace VDS_Backend.Src.Models.VDS
{
    /// <summary>
    /// The interface between the database and the server.
    /// only implements the necessary interface methods: that is the methods that define the interface
    /// between the client and the server.
    /// </summary>
    internal class VDbInterface
    {
        /// <summary>
        /// the database handler
        /// </summary>
        private VDbHandler handler;

        /// <summary>
        /// Create a new interface to the database based on a given context.
        /// </summary>
        /// <param name="context">the context of the database</param>
        public VDbInterface(VDSContext context)
        {
            handler = new VDbHandler(context);
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
        public OperationStatus SignUp(string firstName, string lastName, string email, string password, string phoneNumber)
        {
            if(handler.addUser(firstName, lastName, email, password, phoneNumber))
            {
                return OperationStatus.Success;
            }
            return OperationStatus.AlreadyExistsError;
        }


        /// <summary>
        /// Determine if the user email and password exist in the database
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="password">password of the user</param>
        /// <returns>Sucess if logged in successfully, otherwise CredentialsError.</returns>
        public OperationStatus Login(string email, string password)
        {
            // user from db
            User? user = handler.GetUser(email);
            // user does not exist
            if (user == null) { return OperationStatus.CredentialsError; }
            // password mismatch
            if (!user.Password.Equals(password)) { return OperationStatus.CredentialsError; }
            return OperationStatus.Success;
        }
    }
}
