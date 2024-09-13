using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Utilities
{
    /// <summary>
    /// Either success or error category types
    /// </summary>
    internal enum StatusCode
    {
        // successful operation
        Success,
        // result not found
        NotFoundError,
        // couldn't login successfully
        CredentialsError,
        // something already exists in the database.
        AlreadyExistsError,
        // generic server error
        ServerError,
        // given arguments are invalid
        IllegalArgument,
        // something is full and cannot add more things to it
        ObjectFull
    }

    /// <summary>
    /// A pair of a status code and the message related to it
    /// </summary>
    internal class OperationStatus
    {
        public static readonly OperationStatus SUCCESS = new OperationStatus(StatusCode.Success, "Success.");
        public static readonly OperationStatus NOTIFICATION_SEMI_SUCCESS = new OperationStatus(StatusCode.Success, "Successfully edited the post, but not all volunteers received notification.");
        public static readonly OperationStatus SIGNUP_USER_EXISTS_ERROR = new OperationStatus(StatusCode.AlreadyExistsError, "User already exists.");
        public static readonly OperationStatus LOGIN_WRONG_EMAIL_PASSWORD_ERROR = new OperationStatus(StatusCode.CredentialsError, "Wrong email or password.");
        public static readonly OperationStatus UNKNOWN_PAYLOAD_ERROR = new OperationStatus(StatusCode.ServerError, "Failed parse http(s) body.");
        public static readonly OperationStatus FAILED_POST_CREATION_ERROR = new OperationStatus(StatusCode.ServerError, "Failed to create post.");
        public static readonly OperationStatus INVALID_CONNECTION_KEY_ERROR = new OperationStatus(StatusCode.CredentialsError, "Invalid connection key.");
        public static readonly OperationStatus ILLEGAL_DATES_ERROR = new OperationStatus(StatusCode.IllegalArgument, "Initial Date is after last date.");
        public static readonly OperationStatus ILLEGAL_DATE_FILTER_TYPE_ERROR = new OperationStatus(StatusCode.IllegalArgument, "Date Filter Type is neither 'Contains' nor 'Intersects'.");
        public static readonly OperationStatus FAILED_DB_PARSE_ERROR = new OperationStatus(StatusCode.ServerError, "Failed to retreive parsed data from database.");
        public static readonly OperationStatus FAILED_GETTING_FILTERED_POSTS_ERROR = new OperationStatus(StatusCode.ServerError, "Something happened when trying to get filtered posts, my bad.");
        public static readonly OperationStatus FAILED_GETTING_USER_POSTS_ERROR = new OperationStatus(StatusCode.ServerError, "Something happened when trying to get user posts, my bad.");
        public static readonly OperationStatus POST_NOT_FOUND_ERROR = new OperationStatus(StatusCode.NotFoundError, "Couldn't find the specified post from the given connection key and post id.");
        public static readonly OperationStatus POST_FULL_ERROR = new OperationStatus(StatusCode.ObjectFull, "Given Post is already full.");
        public static readonly OperationStatus POST_OR_USER_NOT_FOUND_ERROR = new OperationStatus(StatusCode.NotFoundError, "Given Post or connection key could not be found.");
        public static readonly OperationStatus FAILED_JOINING_USER_TO_POST_ERROR = new OperationStatus(StatusCode.AlreadyExistsError, "Failed to join user to post, or user already has joined the post.");
        public static readonly OperationStatus FAILED_LEAVING_POST_ERROR = new OperationStatus(StatusCode.AlreadyExistsError, "Failed remove user from post, or user already has left the post.");
        public static readonly OperationStatus FAILED_BROADCASTING_MSG_ERROR = new OperationStatus(StatusCode.ServerError, "Failed to broadcast message to ALL volunteers, some MIGHT still have received.");

        public StatusCode Code { get; }
        public string Message { get; }

        private OperationStatus(StatusCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    /// <summary>
    /// thrown whenever there is an attempt to add a user to a post but it's full.
    /// </summary>
    internal class MaxVolunteersReachedException : Exception { }
}
