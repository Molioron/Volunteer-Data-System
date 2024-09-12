using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using VDS_Backend.Src.Utilities;

namespace VDS_Backend.Src.VDSServer
{
    /// <summary>
    /// A class responsible for handling/sending mails to users of the system.
    /// </summary>
    internal class MailHandler : IDisposable
    {
        /// <summary>
        /// the name of the broadcasting email system
        /// </summary>
        public static readonly string SYSTEM_NAME = "VDS-Broadcaster";

        /// <summary>
        /// the smtp client to send emails through
        /// </summary>
        private SmtpClient client;
        /// <summary>
        /// The email address of the smtp server owner
        /// </summary>
        private string smtpOwnerEmail;
        
        public MailHandler(string filepath, string key)
        {
            // the info extracted from the file, throw exception if info is null
            var info = ExtractFields(filepath, key) ?? throw new Exception("Failed to initialize MailHandler.");
            client = new SmtpClient();
            client.Connect(info.SmtpServerService, 465, true); // 465 port is for ssl
            client.Authenticate(info.SmtpOwnerEmail, info.AppPassword);
            smtpOwnerEmail = info.SmtpOwnerEmail;
        }

        /// <summary>
        /// Sends an email to the specified recipient.
        /// </summary>
        /// <param name="recipient">Recipient's email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <returns></returns>
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SYSTEM_NAME, smtpOwnerEmail));
            message.To.Add(MailboxAddress.Parse(recipient));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            // send the email asynchronously
            await client.SendAsync(message);
        }

        public void Dispose()
        {
            client.DisconnectAsync(true);
            client.Dispose();
        }



        /// <summary>
        /// Extracts the information from the given json file to put into mail handler.
        /// The information is: SmtpServerName, AppPassword, SmtpOwnerEmail.
        /// </summary>
        /// <param name="filepath">path to json file to read from</param>
        /// <param name="key">the symmetric key used for encrypting the file's content</param>
        /// <returns></returns>
        private SmtpServerInfo? ExtractFields(string filepath, string key)
        {
            // string in json format of the file text content
            string jsonString;
            try
            {
                string cipherTxt = File.ReadAllText(filepath);
                jsonString = Utils.DecryptString(key, cipherTxt);
            }
            catch (Exception e) { // failed to read from file
                Console.WriteLine($"Failed to read from file: \"{filepath}\"!");
                Console.WriteLine($"{e.Message}");
                return null;
            }
            var res = JsonConvert.DeserializeObject<SmtpServerInfo>(jsonString);

            if(res is null) // failed to deserialize
            {
                Console.WriteLine($"Failed to deserialize json content of \"{filepath}\"");
            }

            return res;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SmtpServerInfo
    {
        /// <summary>
        /// the service address of the smtp server
        /// </summary>
        public string SmtpServerService { get; set;}
        /// <summary>
        /// the password to the mail application server
        /// </summary>
        public string AppPassword { get; set;}

        /// <summary>
        /// the email of the stmp server owner
        /// </summary>
        public string SmtpOwnerEmail { get; set; }
    }
}
