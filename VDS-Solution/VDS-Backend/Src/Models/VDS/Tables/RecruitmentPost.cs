using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.DataTypes;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    /// <summary>
    /// A post a user can post to recruit volunteers to a specific volunteering place.
    /// </summary>
    internal class RecruitmentPost
    {
        public int Id { get; set; } // primary key auto incremented.

        [ForeignKey("User")]
        public string UserEmail { get; set; }

        // navigation property
        public User User { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// General location of where the volunteering place is.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The general type of work in this place.
        /// </summary>
        public Job Job { get; set; }

        /// <summary>
        /// First date one can volunteer in this place.
        /// </summary>
        public DateTime InitialDate { get; set; }

        /// <summary>
        /// Last date one can volunteer in this place.
        /// </summary>
        public DateTime LastDate { get; set; }
    }
}
