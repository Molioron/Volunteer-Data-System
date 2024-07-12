using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.DataTypes;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    internal class VolunteerPost
    {
        public int Id { get; set; } // primary key auto incremented.

        /// <summary>
        /// General locations where the volunteer prefers to volunteer in.
        /// </summary>
        public ICollection<VolunteerPostLocation> Locations { get; set; }

        /// <summary>
        /// The general type of jobs the volunteer prefers to volunteer in.
        /// </summary>
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
