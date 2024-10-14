using System.Security.Cryptography;
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


        /// <summary>
        /// Encrypts a string with a given key
        /// </summary>
        /// <param name="key">symmetric key</param>
        /// <param name="plainText">text to encrypt</param>
        /// <returns>the attempt result of encryption</returns>
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv;
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key); // convert key back from base64 to byte[]
                aes.GenerateIV();
                iv = aes.IV; // Randomly generate an IV

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            // Prepend the IV to the encrypted data
            byte[] result = new byte[iv.Length + array.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(array, 0, result, iv.Length, array.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts a given string from a given key
        /// </summary>
        /// <param name="key">symmetric key</param>
        /// <param name="cipherText">ciphered text to decrypt</param>
        /// <returns>the attempt result of decrypting the string</returns>
        public static string DecryptString(string key, string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            // Extract the IV from the fullCipher (first 16 bytes)
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            // Extract the actual cipher (the rest of the data)
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key); // convert key back from base64 to byte[]
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipher))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a secure random string key of specified length
        /// </summary>
        /// <param name="lengthInBytes">length of the generated key</param>
        /// <returns>the generated key</returns>
        /// <exception cref="ArgumentException">if the length is a non positive number</exception>
        public static string GenerateSecureKey(int lengthInBytes)
        {
            if (lengthInBytes <= 0)
                throw new ArgumentException("Length must be a positive number", nameof(lengthInBytes));

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[lengthInBytes];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        /// <summary>
        /// Generates key and encrypts json value from given credentials. prints to console result.
        /// </summary>
        /// <param name="smtpServerService">name of service</param>
        /// <param name="appPassword">password of app of service</param>
        /// <param name="smtpOwnerEmail">email of owner in service</param>
        public static void GenerateCredentialsEncrypted(string smtpServerService,
    string appPassword, string smtpOwnerEmail)
        {
            const int KEY_LENGTH = 32;
            string text = $"{{\"SmtpServerService\": \"{smtpServerService}\"," +
                $" \"AppPassword\": \"{appPassword}\", \"SmtpOwnerEmail\": \"{smtpOwnerEmail}\"}}";
            string key = GenerateSecureKey(KEY_LENGTH);
            Console.WriteLine($"original text: {text},\nkey: {key},");
            string encrypted = EncryptString(key, text);

            Console.WriteLine($"encrypted text: {encrypted}");
        }
    }
}
