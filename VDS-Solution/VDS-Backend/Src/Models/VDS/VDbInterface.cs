using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
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
            // post added successfully
            if (handler.addUser(firstName, lastName, email, password, phoneNumber))
            {
                return OperationStatus.SUCCESS;
            }
            return OperationStatus.SIGNUP_USER_EXISTS_ERROR;
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
            if (user == null) { return OperationStatus.LOGIN_WRONG_EMAIL_PASSWORD_ERROR; }
            // password mismatch
            if (!user.Password.Equals(password)) { return OperationStatus.LOGIN_WRONG_EMAIL_PASSWORD_ERROR; }
            return OperationStatus.SUCCESS;
        }

        /// <summary>
        /// Creates a new recruitment post for a given user.
        /// </summary>
        /// <param name="email">the user who owns the post</param>
        /// <param name="title">the title of the post</param>
        /// <param name="description">the description of the post</param>
        /// <param name="address">the address of the place of volunteering</param>
        /// <param name="volunteerArea">the general location of the place of volunteering</param>
        /// <param name="jobType">the general job/work in the place of volunteering</param>
        /// <param name="initialDate">initial date of the duration of volunteering</param>
        /// <param name="lastDate">last date of the duration of volunteering</param>
        /// <returns>Sucess if the post was created successfully, otherwise ServerError.</returns>
        public OperationStatus CreatePost(string email, string title, string description,
            string address, Location volunteerArea, Job jobType, DateTime initialDate, DateTime lastDate)
        {
            if(handler.addRecruitmentPost(email,title, description, address, volunteerArea, jobType,
                initialDate, lastDate))
            {
                return OperationStatus.SUCCESS; // post added successfully
            }
            return OperationStatus.FAILED_POST_CREATION_ERROR; // for some reason could not create post.
        }
    }
}
