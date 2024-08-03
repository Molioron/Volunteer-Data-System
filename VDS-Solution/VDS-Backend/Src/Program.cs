// See https://aka.ms/new-console-template for more information
using SQLitePCL;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;


// Initialize SQLitePCL
Batteries_V2.Init();

await using VDSContext db = new VDSContextSQLite();
VDbHandler handler = new VDbHandler(db);



Console.WriteLine("adding data");
handler.addUser("John", "Doe", "JohnDoe@gmail.com", "JohnDoe123", "0000000000");
handler.removeUser("JaneDoe@gmail.com");
var post_add_res = handler.addRecruitmentPost("JohnDoe@gmail.com", "Haifa tour instructor needed", "instructor needed asap", "Somewhere in Haifa",
    Location.North, Job.Transportation, DateTime.Now, DateTime.Now.AddDays(1));
Console.WriteLine($"result of adding post: {post_add_res}");
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