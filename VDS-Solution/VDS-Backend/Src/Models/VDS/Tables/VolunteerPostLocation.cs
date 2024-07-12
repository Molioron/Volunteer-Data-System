using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.DataTypes;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    /// <summary>
    /// Alows a volunteer post to have multiple types of prefered general locations.
    /// </summary>
    [PrimaryKey(nameof(Location), nameof(VolunteerPost))]
    internal class VolunteerPostLocation
    {
        public Location Location { get; set; }

        public VolunteerPost VolunteerPost { get; set; }
    }
}
