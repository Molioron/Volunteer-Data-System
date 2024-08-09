// See https://aka.ms/new-console-template for more information
using SQLitePCL;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
using VDS_Backend.Src.VDSServer;


// Initialize SQLitePCL
Batteries_V2.Init();

await using VDSContext db = new VDSContextSQLite();
ServerInterface serverInterface = new ServerInterface(db);

serverInterface.SignUp("Jane", "Doe", "JaneDoe@gmail.com", "Jane123", "1111111111");
string key = serverInterface.Login("JaneDoe@gmail.com", "Jane123");
serverInterface.Login("JohnDoe@gmail.com", "JohnDoe123");
serverInterface.Login("JaneDoe@gmail.com", "wrongPass");
serverInterface.Login("wrongMail", "Jane123");
serverInterface.CreatePost("11111", "Bus driver needed", "looking for a bus driver in haifa.",
    "somewhere in Haifa", Location.North, Job.Transportation, DateTime.Now, DateTime.Now.AddDays(1));

//ServerApp.Run("localhost", 9000);