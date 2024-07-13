using Microsoft.EntityFrameworkCore;
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
    /// Alows a volunteer post to have multiple types of prefered jobs
    /// </summary>
    [PrimaryKey(nameof(Job), nameof(VolunteerPostId))]
    internal class VolunteerPostJob
    {
        public Job Job { get; set; }

        [ForeignKey("VolunteerPost")]
        public int VolunteerPostId { get; set; }

        // navigation property

        public VolunteerPost VolunteerPost { get; set; }
    }
}
