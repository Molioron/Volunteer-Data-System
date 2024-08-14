using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Models.VDS.DataTypes
{
    /// <summary>
    /// The type of filtering operation:
    /// Contains - date is fully contained within range of dates
    /// Intersects - date intersects the range of dates
    /// </summary>
    internal enum DateFilterType
    {
        Contains,
        Intersects
    }
}
