using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Utilities
{
    internal enum OperationStatus
    {
        // successful operation
        Success,
        // result not found
        NotFoundError,
        // connection key is invalid or couldn't login successfully
        CredentialsError,
        // something already exists in the database.
        AlreadyExistsError,
        // generic server error
        ServerError,
    }
}
