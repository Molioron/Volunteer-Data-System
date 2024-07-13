using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src
{
    internal class Utils
    {
        /// <summary>
        /// Makes an absolute path from a given relative path such that the relative path is relative to the project's root.
        /// </summary>
        /// <param name="relative_path">relative path to the project's root</param>
        /// <returns>the absolute path</returns>
        public static string AbsPathFromRoot(string relative_path)
        {
            string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            string dbPath = Path.Combine(projectRootPath, relative_path);

            return dbPath;
        }
    }
}
