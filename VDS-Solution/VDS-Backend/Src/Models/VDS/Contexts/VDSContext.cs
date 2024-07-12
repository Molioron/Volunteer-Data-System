using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.Tables;

namespace VDS_Backend.Src.Models.VDS.Contexts
{
    /// <summary>
    /// A Facade to the database context to the volunteer system.
    /// facade in a sense that it should not be instanciated and instead a concrete class should
    /// inherit from this class to specify what can of database is created (e.g: sqlite, postregsql, etc...)
    /// </summary>
    internal abstract class VDSContext : DbContext
    {
        /// <summary>
        /// The table of users in the system.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// The table of posts about recruiting volunteers in the system.
        /// </summary>
        public DbSet<RecruitmentPost> Recruitments { get; set; }

        /// <summary>
        /// The table of posts about volunteering somewhere in the system.
        /// </summary>
        public DbSet<VolunteerPost> Volunteers { get; set; }
    }
}
