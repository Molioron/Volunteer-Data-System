using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Utilities;

namespace VDS_Backend.Src.Models.VDS.Contexts
{
    /// <summary>
    /// A concrete implementation of the context of the volunteer system database.
    /// This implementation is specifically for SQLite.
    /// </summary>
    internal class VDSContextSQLite : VDSContext
    {
        private const string DB_NAME = @"volunteer_data_system.db";
        private const string DB_PATH_RELATIVE = $@"Assets\Databases\{DB_NAME}";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Utils.AbsPathFromRoot(DB_PATH_RELATIVE);

            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}
