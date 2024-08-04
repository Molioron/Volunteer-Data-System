// See https://aka.ms/new-console-template for more information
using SQLitePCL;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
using VDS_Backend.Src.Server;


// Initialize SQLitePCL
Batteries_V2.Init();

await using VDSContext db = new VDSContextSQLite();
ServerInterface serverInterface = new ServerInterface(db);

serverInterface.SignUp("Jane", "Doe", "JaneDoe@gmail.com", "Jane123", "1111111111");
serverInterface.Login("JaneDoe@gmail.com", "Jane123");
serverInterface.Login("JohnDoe@gmail.com", "JohnDoe123");
serverInterface.Login("JaneDoe@gmail.com", "wrongPass");
serverInterface.Login("wrongMail", "Jane123");

var results_all = from user in db.Users
              select user;

var results = from user in db.Users
             where user.FirstName == "John"
             select user;

var posts = from recruitment in db.Recruitments
            select recruitment;

var post_owners = from recruitment in db.Recruitments
                 where recruitment.Id == 1
                 select recruitment.User;

Console.WriteLine("finding everyone:");
foreach (var result in results_all)
{
    Console.WriteLine($"{result.FirstName} {result.LastName}, mail: {result.Email}, phone: {result.PhoneNumber}");
}

Console.WriteLine("finding john:");
foreach (var result in results)
{
    Console.WriteLine($"{result.FirstName} {result.LastName}, mail: {result.Email}, phone: {result.PhoneNumber}");
}

Console.WriteLine("finding posts:");
foreach (var result in posts)
{
    Console.WriteLine($"{result.Id}, {result.User.FirstName} {result.User.LastName}, {result.UserEmail}, {result.Title}, {result.Description}");
}

Console.WriteLine("finding post owner:");
foreach (var result in post_owners)
{
    Console.WriteLine($"{result.FirstName} {result.LastName}");
}