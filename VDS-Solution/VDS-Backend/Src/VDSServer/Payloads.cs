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
        /// input received from logout (and also get user posts)
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

            public int MaxVolunteers { get; set; }
        }

        /// <summary>
        /// input received from filtered posts request
        /// </summary>
        private class GetFilteredPostsInputPayload
        {
            public string ConnectionKey { get; set; }
            public Location[] VolunteerAreas {  get; set; }
            public Job[] JobTypes {  get; set; }
            public DateTime? InitialDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateFilterType? DateFilterType { get; set; }
        }

        /// <summary>
        /// input received from edit post request
        /// </summary>
        private class EditPostInputPayload
        {
            public string ConnectionKey { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Address { get; set; }
            public Location VolunteerArea { get; set; }
            public Job JobType { get; set; }
            public DateTime InitialDate { get; set; }
            public DateTime LastDate { get; set; }
        }

        /// <summary>
        /// input received from delete post request,
        /// as well as any requests that only requrie connection key and post id
        /// </summary>
        private class ConnectionKeyPostInputPayload
        {
            public string ConnectionKey { get; set; }
            public int Id { get; set; }
        }
    }
}
