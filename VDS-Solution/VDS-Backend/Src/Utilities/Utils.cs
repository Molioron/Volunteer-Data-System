using System.Text;

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

        /// <summary>
        /// Generates a random string of characters from a given length.
        /// </summary>
        /// <param name="length">the length of the random string</param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a given hour and minute at local time, to the time at UTC time zone.
        /// </summary>
        /// <param name="hour">specified hour in local time</param>
        /// <param name="minute">specified minute in local time</param>
        /// <returns>The time of the specified hour and minute in UTC timezone.</returns>
        public static DateTime ConvertToUtc(int hour, int minute)
        {
            // Get the current date and time in the local time zone
            DateTime localTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hour,
                minute,
                0, // seconds
                DateTimeKind.Local // Specifies that this time is local
            );

            // Convert the local time to UTC
            DateTime utcTime = localTime.ToUniversalTime();

            return utcTime;
        }
    }
}
