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
    /// Alows a volunteer post to have multiple types of prefered jobs
    /// </summary>
    [PrimaryKey(nameof(Job), nameof(VolunteerPost))]
    internal class VolunteerPostJob
    {
        public Job Job { get; set; }

        public VolunteerPost VolunteerPost { get; set; }
    }
}
