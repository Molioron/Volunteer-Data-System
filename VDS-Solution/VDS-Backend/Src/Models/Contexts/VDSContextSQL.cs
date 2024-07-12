using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Models.Contexts
{
    /// <summary>
    /// A concrete implementation of the context of the volunteer system database.
    /// This implementation is specifically for SQLite.
    /// </summary>
    internal class VDSContextSQL : VDSContext
    {
        private const string DB_NAME = @"volunteer_data_system.db";
        private const string DB_PATH = $@"Assets\Databases\{DB_NAME}";

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DB_PATH}");
    }
}
