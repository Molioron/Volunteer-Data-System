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
    }

    /// <summary>
    /// A pair of a status code and the message related to it
    /// </summary>
    internal class OperationStatus
    {
        public static readonly OperationStatus SUCCESS = new OperationStatus(StatusCode.Success, "Success.");
        public static readonly OperationStatus SIGNUP_USER_EXISTS_ERROR = new OperationStatus(StatusCode.AlreadyExistsError, "User already exists.");
        public static readonly OperationStatus LOGIN_WRONG_EMAIL_PASSWORD_ERROR = new OperationStatus(StatusCode.CredentialsError, "Wrong email or password.");
        public static readonly OperationStatus UNKNOWN_PAYLOAD_ERROR = new OperationStatus(StatusCode.ServerError, "Failed parse http(s) body.");
        public static readonly OperationStatus FAILED_POST_CREATION_ERROR = new OperationStatus(StatusCode.ServerError, "Failed to create post.");
        public static readonly OperationStatus INVALID_CONNECTION_KEY_ERROR = new OperationStatus(StatusCode.CredentialsError, "Invalid connection key.");
        public static readonly OperationStatus ILLEGAL_DATES_ERROR = new OperationStatus(StatusCode.IllegalArgument, "Initial Date is after last date.");

        public StatusCode Code { get; }
        public string Message { get; }

        private OperationStatus(StatusCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
