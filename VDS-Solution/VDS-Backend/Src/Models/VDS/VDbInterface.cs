using System.Net;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
using VDS_Backend.Src.Models.VDS.Tables;
using VDS_Backend.Src.Utilities;

namespace VDS_Backend.Src.Models.VDS
{
    /// <summary>
    /// The interface between the database and the server interface.
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
        /// <param name="maxVolunteers">maximum number of volunteers allowed to join post</param>
        /// <returns>Sucess if the post was created successfully, otherwise ServerError.</returns>
        public OperationStatus CreatePost(string email, string title, string description,
            string address, Location volunteerArea, Job jobType, DateTime initialDate, DateTime lastDate,
            int maxVolunteers)
        {
            try
            {
                if (handler.AddRecruitmentPost(email, title, description, address, volunteerArea, jobType,
                initialDate, lastDate, maxVolunteers))
                {
                    return OperationStatus.SUCCESS; // post added successfully
                }
            }
            catch (ArgumentException ex) { return OperationStatus.ILLEGAL_DATES_ERROR; }
            return OperationStatus.FAILED_POST_CREATION_ERROR; // for some reason could not create post.
        }

        /// <summary>
        /// gets filtered posts from the db.
        /// if any of the options are empty/null they will be ignored.
        /// </summary>
        /// <param name="volunteerAreas">allowed locations</param>
        /// <param name="jobTypes">allowed jobs</param>
        /// <param name="initialDate">initial date to look for</param>
        /// <param name="endDate">last date to look for</param>
        /// <param name="dateFilterType">type of date filtering: Contains or Intersects</param>
        /// <param name="email"> the email of the user getting filtered posts</param>
        /// <returns>tuple (status, filtered posts)</returns>
        public (OperationStatus, VDbHandler.PostInfo[]) GetFilteredPosts(Location[] volunteerAreas,
            Job[] jobTypes, DateTime? initialDate, DateTime? endDate, DateFilterType? dateFilterType, string email)
        {
            try
            {
                var result = handler.GetFilteredPosts(volunteerAreas, jobTypes, initialDate, endDate, dateFilterType, email);
                return (OperationStatus.SUCCESS, result);
            }
            catch (NotImplementedException)
                { return (OperationStatus.ILLEGAL_DATE_FILTER_TYPE_ERROR, []); }
            catch (ArgumentNullException)
            { return (OperationStatus.FAILED_DB_PARSE_ERROR, []); }
            catch (ArgumentException e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
                return (OperationStatus.ILLEGAL_DATES_ERROR, []); }
            catch (Exception ex) { Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return (OperationStatus.FAILED_GETTING_FILTERED_POSTS_ERROR, []);
            }
        }

        /// <summary>
        /// Get all posts made by a user
        /// </summary>
        /// <param name="email">email of user</param>
        /// <returns>tuple (status, user posts)</returns>
        public (OperationStatus, VDbHandler.PostInfo[]) GetUserPosts(string email)
        {
            try
            {
                var post = handler.GetUserPosts(email);
                return (OperationStatus.SUCCESS, post);
            }
            catch(Exception ex)
            {
                return (OperationStatus.FAILED_GETTING_USER_POSTS_ERROR, []);
            }
        }

        /// <summary>
        /// Edit a post with new fields
        /// </summary>
        /// <param name="email">user owner email</param>
        /// <param name="id">post id</param>
        /// <param name="title">new title</param>
        /// <param name="description">new description</param>
        /// <param name="address">new address</param>
        /// <param name="volunteerArea">new volunteerArea</param>
        /// <param name="jobType">new jobType</param>
        /// <param name="initialDate">new initial date</param>
        /// <param name="endDate">new end date</param>
        /// <returns>success or post not found error</returns>
        public OperationStatus EditPost(string email, int id, string title,
            string description, string address, Location volunteerArea, Job jobType, DateTime initialDate, DateTime endDate)
        {
            // post editted successfully
            if (handler.EditPost(email, id, title, description, address, volunteerArea, jobType,
                initialDate, endDate))
            {
                return OperationStatus.SUCCESS;
            }
            return OperationStatus.POST_NOT_FOUND_ERROR;
        }

        /// <summary>
        /// Delete a post with id, makes sure email matches the user owns the post
        /// </summary>
        /// <param name="email">user email who owns the post</param>
        /// <param name="id">post id</param>
        /// <returns>success, or post not found</returns>
        public OperationStatus DeletePost(string email, int id)
        {
            // post edited successfully
            if (handler.FindPostWithOwner(email, id))
            {
                // attempt deletion only if post with owner
                if (handler.removeRecruitmentPost(id))
                {
                    return OperationStatus.SUCCESS;
                }
            }
            return OperationStatus.POST_NOT_FOUND_ERROR;
        }

        /// <summary>
        /// Allows a user to join a post.
        /// </summary>
        /// <param name="email">email of user</param>
        /// <param name="postId">id of post to join</param>
        /// <returns>true if successfully user has joined post, otherwise false if user already joined post
        /// or an error has occurred along the way.</returns>
        public OperationStatus JoinUserToPost(string email, int postId)
        {
            try
            {
                if (handler.JoinUserToPost(email, postId))
                {
                    return OperationStatus.SUCCESS;
                }
            }
            catch(ArgumentException) { return OperationStatus.POST_OR_USER_NOT_FOUND_ERROR; }
            catch(MaxVolunteersReachedException) { return OperationStatus.POST_FULL_ERROR; }
            return OperationStatus.FAILED_JOINING_USER_TO_POST;
        }

        /// <summary>
        /// Find and removes all posts in the database that have expired.
        /// That is, their last date is older than the current day.
        /// Implemented in VDbInterface too, to keep the layer architecture consistent.
        /// </summary>
        public void DeleteExpiredPosts()
        {
            handler.DeleteExpiredPosts();
        }
    }

}
