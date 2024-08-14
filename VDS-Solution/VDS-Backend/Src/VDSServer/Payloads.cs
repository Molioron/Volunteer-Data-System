using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.DataTypes;

namespace VDS_Backend.Src.VDSServer
{
    /*
     Contains types of payloads (http json body) the server receives
     */
    internal partial class ServerInterface
    {
        /// <summary>
        /// input receives from signup
        /// </summary>
        private class SignupInputPayload
        {
            public string FirstName {  get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone {  get; set; }
        }

        /// <summary>
        /// input receives from login
        /// </summary>
        private class LoginInputPayload
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// input received from logout
        /// </summary>
        private class LogoutInputPayload
        {
            public string ConnectionKey { get; set; }
        }

        /// <summary>
        /// input received from create post request
        /// </summary>
        private class CreatePostInputPayload
        {
            public string ConnectionKey {  get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Address { get; set; }
            public Location VolunteerArea { get; set; }
            public Job JobType { get; set; }
            public DateTime InitialDate { get; set; }
            public DateTime LastDate { get; set; }
        }
    }
}
