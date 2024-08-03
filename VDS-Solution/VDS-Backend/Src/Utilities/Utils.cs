using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Utilities
{
    internal class Utils
    {
        /// <summary>
        /// randomness generator
        /// </summary>
        private static readonly Random random = new Random();
        // all possible characters for random string generation
        private const string ALL_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateRandomString(int length)
        {
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(ALL_CHARS[random.Next(ALL_CHARS.Length)]);
            }
            return stringBuilder.ToString();
        }

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
