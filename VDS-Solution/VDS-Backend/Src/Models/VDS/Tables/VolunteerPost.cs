using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.DataTypes;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    internal class VolunteerPost
    {
        // primary key auto incremented.
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserEmail { get; set; }

        // navigation property
        public User User { get; set; }

        /// <summary>
        /// General locations where the volunteer prefers to volunteer in.
        /// </summary>
        // navigation property
        public ICollection<VolunteerPostLocation> Locations { get; set; }

        /// <summary>
        /// The general type of jobs the volunteer prefers to volunteer in.
        /// </summary>
        // navigation property
        public ICollection<VolunteerPostJob> Jobs { get; set; }

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
