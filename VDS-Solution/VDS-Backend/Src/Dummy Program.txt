// See https://aka.ms/new-console-template for more information
using SQLitePCL;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.Tables;


// Initialize SQLitePCL
Batteries_V2.Init();

Console.WriteLine("Hello, World!");

await using VDSContext db = new VDSContextSQLite();

db.Database.EnsureCreated();

Console.WriteLine("adding data");
User u1 = new User()
{
    FirstName = "John",
    LastName = "Doe",
    Email = "JohnDoe@gmail.com",
    Password = "JohnDoe123",
    PhoneNumber = "0000000000",
    RecruitmentPosts = [],
    VolunteerPosts = []
};
User u2 = new User()
{
    FirstName = "Jane",
    LastName = "Doe",
    Email = "JaneDoe@gmail.com",
    Password = "JaneDoe123",
    PhoneNumber = "1111111111",
    RecruitmentPosts = [],
    VolunteerPosts = []
};
VDbHandler.AddIfNotExists(db.Users, u1);
VDbHandler.AddIfNotExists(db.Users, u2);

db.SaveChanges();

var results_all = from user in db.Users
              select user;

var results = from user in db.Users
             where user.FirstName == "John"
             select user;

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