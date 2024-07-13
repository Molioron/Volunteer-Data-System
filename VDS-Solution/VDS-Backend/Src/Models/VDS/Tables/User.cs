using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    /// <summary>
    /// A user in the system that can be both a volunteer and a recruiter.
    /// A user can create posts where they require assistance and thus recruit volunteers,
    /// or posts where volunteer to help other users.
    /// </summary>
    [PrimaryKey(nameof(Email))]
    internal class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; } // primary key
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        // user owned recruitment posts.
        // navigation property
        public ICollection<RecruitmentPost> RecruitmentPosts { get; set; }
        // user owned volunteer posts.
        // navigation property
        public ICollection<VolunteerPost> VolunteerPosts { get; set; }
    }
}
