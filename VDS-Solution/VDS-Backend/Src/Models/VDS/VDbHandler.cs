using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.Tables;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VDS_Backend.Src.Models.VDS.DataTypes;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Net;
using VDS_Backend.Src.Utilities;
using Microsoft.EntityFrameworkCore.Query;

namespace VDS_Backend.Src.Models.VDS
{
    /// <summary>
    /// A class responsible for making queries and adding entities to the database.
    /// </summary>
    internal class VDbHandler
    {
        // db context
        private VDSContext Context { get; set; }

        /// <summary>
        /// Creates a new handler for the given context.
        /// Ensures that the database is created.
        /// </summary>
        /// <param name="db">given context to the database.</param>
        public VDbHandler(VDSContext db) 
        {
            Context = db;
            Context.Database.EnsureCreated();
        }

        /// <summary>
        /// Adds a user to the database from given input
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>true, if the user was added successfully, otherwise false.</returns>
        public bool addUser(String firstName, String lastName, string email, string password, string phoneNumber)
        {
            User u = new User()
            { FirstName = firstName, LastName = lastName,Email = email, Password = password, PhoneNumber = phoneNumber,
            RecruitmentPosts = []};

            return AddIfNotExists(Context.Users, u) is not null;
        }
        /// <summary>
        /// Removes a user from the database based on it's email primary key
        /// </summary>
        /// <param name="email">the email of the user.</param>
        /// <returns>true if the user was removed successfully, otherwise false.</returns>
        public bool removeUser(string email)
        {
            return RemoveIfExists(Context.Users, email);
        }
        /// <summary>
        /// Creates a new recruitment post for the user.
        /// </summary>
        /// <param name="userEmail">the user who owns the post</param>
        /// <param name="title">the title of the post</param>
        /// <param name="description">the description of the post</param>
        /// <param name="address">the address of the place of volunteering</param>
        /// <param name="location">the general location of the place of volunteering</param>
        /// <param name="job">the general job/work in the place of volunteering</param>
        /// <param name="initialDate">initial date of the duration of volunteering</param>
        /// <param name="lastDate">last date of the duration of volunteering</param>
        /// <param name="maxVolunteers">maximum number of volunteers allowed to join post</param>
        /// <returns>true if the post was created successfully, otherwise false.</returns>
        /// <exception cref="ArgumentException">if initial date is after last date.</exception>
        public bool AddRecruitmentPost(string userEmail, string title, string description, string address,
            Location location, Job job, DateTime initialDate, DateTime lastDate, int maxVolunteers)
        {
            if (initialDate > lastDate) // sanity check: initial date is not after last date.
            {
                throw new ArgumentException("Specified initialDate is after lastDate!");
            }

            var post = new RecruitmentPost()
            { UserEmail = userEmail, Title = title, Description = description, Address = address, Location = location,
            Job = job, InitialDate = initialDate, LastDate = lastDate, MaxVolunteers = maxVolunteers};
            return AddIfNotExists(Context.Recruitments, post) is not null;
        }
        /// <summary>
        /// Removes a recruitment post from the database based on it's id primary key.
        /// </summary>
        /// <param name="id">the id of the post.</param>
        /// <returns>true if the post was removed successfully, otherwise false.</returns>
        public bool removeRecruitmentPost(int id)
        {
            return RemoveIfExists(Context.Recruitments, id);
        }

        /// <summary>
        /// determines if a post exists with the given id and email of owner
        /// </summary>
        /// <param name="email">user owner email</param>
        /// <param name="id">post id</param>
        /// <returns>true if it exists, otherwise false.</returns>
        public bool FindPostWithOwner(string email, int id)
        {
            var post = Context.Recruitments.SingleOrDefault(posts => posts.UserEmail == email && posts.Id == id);
            return post is not null;
        }

        /// <summary>
        /// Return all posts that have one of the specified volunteerAreas,
        /// one of the specified jobs, and either dates range is contained in the specified range
        /// or intersects with, dependent on the DateFilterType.
        /// no locations, no jobs, or null dates / date filter type, then the method
        /// will ignore these filtering options. 
        /// </summary>
        /// <param name="volunteerAreas">posts must have one of the locations specified</param>
        /// <param name="jobTypes">posts must have one of the jobs specified</param>
        /// <param name="initialDate">initial date specified</param>
        /// <param name="endDate">end date specified</param>
        /// <param name="dateFilterType">type of date filtering (Contains, or Intersects)</param>
        /// <returns>An array of posts that satisfy the specified filter</returns>
        /// <exception cref="NotImplementedException">if dateFilterType is neither Contains or Intersects</exception>
        /// <exception cref="ArgumentException">if initialDate is after endDate</exception>
        /// /// <exception cref="ArgumentNullException">failed to parse data from db</exception>
        public PostInfo[] GetFilteredPosts(Location[] volunteerAreas,
            Job[] jobTypes, DateTime? initialDate, DateTime? endDate, DateFilterType? dateFilterType, string email)
        {
            if (initialDate is not null && endDate is not null)
            {
                if (initialDate > endDate) // sanity check: initial date must not be after end date
                { throw new ArgumentException("initialDate must not be after endDate"); }
            }

            // only posts
            var query = Context.Recruitments.Include(r => r.User).AsQueryable();

            // Create a list to hold all conditions
            var conditions = new List<Expression<Func<RecruitmentPost, bool>>>();

            // filter by location
            if (volunteerAreas.Length > 0) // filter only when necessary
            {
                conditions.Add(posts => volunteerAreas.Contains(posts.Location));
            }
            // filter by job type
            if (jobTypes.Length > 0) // filter only when necessary
            {
                conditions.Add(posts => jobTypes.Contains(posts.Job));
            }

            // filter by date only when necessary (all not null)
            if (initialDate is not null && endDate is not null && dateFilterType is not null)
            {
                // filter by date according to dateFilterType
                switch (dateFilterType)
                {
                    case DateFilterType.Contains:
                        conditions.Add(posts => initialDate <= posts.InitialDate &&
                        posts.InitialDate <= posts.LastDate &&
                        posts.LastDate <= endDate);
                        break;

                    case DateFilterType.Intersects:
                        conditions.Add(posts => posts.InitialDate <= posts.LastDate &&
                                                     ((initialDate <= posts.LastDate && posts.LastDate <= endDate) ||
                                                      (initialDate <= posts.InitialDate && posts.InitialDate <= endDate)) ||
                                                      (posts.InitialDate <= initialDate && endDate <= posts.LastDate));
                        break;

                    default:
                        throw new NotImplementedException("Unknown DateFilterType value.");
                }
            }

            // Apply all conditions in a single Where clause
            var finalCondition = CombineConditions(conditions);
            query = query.Where(finalCondition);

            // determines if the user that used the get filtered posts method,
            // also joined the given post with given id.
            // true if the user has joined the post, otherwise false.
            Func<int, bool> isUserInPost = (int postId) =>
            { 
                var uipQuery = from upr in Context.UserPostRelation
                               where upr.PostId == postId && upr.UserEmail == email
                               select upr;
                return uipQuery.Any();
            };


            // posts as list and out of query.
            var postsList = query.ToList();
            // result of the query
            var result = postsList.Select(posts => new PostInfo
            {
                Id = posts.Id,
                Title = posts.Title,
                Description = posts.Description,
                Address = posts.Address,
                Location = posts.Location,
                Job = posts.Job,
                InitialDate = posts.InitialDate,
                LastDate = posts.LastDate,
                PhoneNumber = posts.User.PhoneNumber,
                IsUserInPost = isUserInPost(posts.Id) // this can only be processed in memory, not in query.

            }).ToArray();
            return result;
        }

        /// <summary>
        /// Returns all posts currently saved, and made by a specific user.
        /// </summary>
        /// <param name="email">email of the user who owns the posts.</param>
        /// <returns>all posts currently saved, and made by a specific user.</returns>
        public PostInfo[] GetUserPosts(string email)
        {
            var query = from posts in Context.Recruitments.Include(r => r.User)
                        where posts.User.Email == email
                        select new PostInfo
                        {
                            Id = posts.Id,
                            Title = posts.Title,
                            Description = posts.Description,
                            Address = posts.Address,
                            Location = posts.Location,
                            Job = posts.Job,
                            InitialDate = posts.InitialDate,
                            LastDate = posts.LastDate,
                            PhoneNumber = posts.User.PhoneNumber
                        };
            return query.ToArray();
        }

        /// <summary>
        /// Update the post of a user witha specific id.
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="id">post id</param>
        /// <param name="title">new title</param>
        /// <param name="description">new description</param>
        /// <param name="address">new address</param>
        /// <param name="volunteerArea">new volunteerArea</param>
        /// <param name="jobType">new jobType</param>
        /// <param name="initialDate">new initialDate</param>
        /// <param name="endDate">new endDate</param>
        /// <returns>true if the post was edited successfully, otherwise false.</returns>
        public bool EditPost(string email, int id, string title,
            string description, string address, Location volunteerArea, Job jobType, DateTime initialDate, DateTime endDate)
        {
            try
            {
                // makes sure there is a post of the same id that the user owns.
                var post = Context.Recruitments.SingleOrDefault(posts => posts.UserEmail == email && posts.Id == id);
                
                if( post == null) { return false; }
                // update the post
                post.Title = title;
                post.Description = description;
                post.Address = address;
                post.Location = volunteerArea;
                post.Job = jobType;
                post.InitialDate = initialDate;
                post.LastDate = endDate;

                Context.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Returns a user from the database from a given email
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>the user matching the given email, otherwise null if not found.</returns>
        public User? GetUser(string email)
        {
            var query = from user in Context.Users
                        where user.Email == email
                        select user;
            if (query.Count() > 0)
            {
                return query.First(); // exactly 1 result
            }
            return null;
        }

        /// <summary>
        /// Determines the number of users who joined a specific post.
        /// </summary>
        /// <param name="postId">id of the post</param>
        /// <returns>the number of users who joined a specific post.
        /// if post id is not valid, it will still return 0.</returns>
        public int GetPostUsersCount(int postId)
        {
            var query = from userPostPair in Context.UserPostRelation
                        where userPostPair.PostId == postId
                        select userPostPair;
            return query.Count();
        }

        /// <summary>
        /// Determines the maximum number of users who can join a specific post.
        /// Throws ArgumentOutofRangeException if no such post exists
        /// </summary>
        /// <param name="postId">id of the post</param>
        /// <returns>the maximum number of users who can join a specific post.</returns>
        public int GetPostMaxUsers(int postId)
        {
            var query = from post in Context.Recruitments
                        where post.Id == postId
                        select post.MaxVolunteers;
            return query.ElementAt(0);
        }

        /// <summary>
        /// Determines if a given user has joined the specified post
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="postId">id of the post</param>
        /// <returns>true if the user has joined the post, otherwise false (even if user or post do not exists).</returns>
        public bool HasUserJoinedPost(string email, int postId)
        {
            var query = from userPostPair in Context.UserPostRelation
                        where userPostPair.UserEmail == email && userPostPair.PostId == postId
                        select userPostPair;
            return query.Any();
        }

        /// <summary>
        /// Allows a user to join a post.
        /// Throws ArgumentException if either post id does not exist.
        /// Throws MaxVolunteersReachedException if the post is full.
        /// </summary>
        /// <param name="email">email of user</param>
        /// <param name="postId">id of post to join</param>
        /// <returns>true if successfully user has joined post, otherwise false if user already joined post
        /// or an error has occurred along the way.</returns>
        public bool JoinUserToPost(string email, int postId)
        {
            if (HasUserJoinedPost(email, postId)) { return false; };
            
            // post id does not exist
            var query1 = from posts in Context.Recruitments
                        where posts.Id == postId
                        select posts;
            if (!query1.Any()) { throw new ArgumentException("post id is invalid!"); }

            int max = GetPostMaxUsers(postId); // should not throw exception because post id passed check.
            int current = GetPostUsersCount(postId);
            // post is full (negative max means no max limit)
            if(max > 0 && current >= max) { throw new MaxVolunteersReachedException(); }

            try
            {
                var relation = new UserPostRelation()
                {
                    UserEmail = email,
                    PostId = postId
                };
                AddIfNotExists(Context.UserPostRelation, relation);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Allows a user to leave a post.
        /// Throws ArgumentException if either post id or user email do not exist.
        /// </summary>
        /// <param name="email">email of user</param>
        /// <param name="postId">id of post to join</param>
        /// <returns>true if successfully user has left post, otherwise false if user hasn't joined the post</returns>
        public bool LeavePost(string email, int postId)
        {
            if (!HasUserJoinedPost(email, postId)) { return false; };

            // post id does not exist
            var query1 = from posts in Context.Recruitments
                         where posts.Id == postId
                         select posts;
            if (!query1.Any()) { throw new ArgumentException("post id is invalid!"); }

            return RemoveIfExists(Context.UserPostRelation, email, postId);
        }

        /// <summary>
        /// returns the volunteers that joined a post.
        /// Does NOT verify that user is owner of post.
        /// Does NOT verify that the caller is the owner or postId is real.
        /// </summary>
        /// <param name="postId">id of post to join</param>
        /// <returns>list of volunteers that joined the post.</returns>
        public VolunteerInfo[] ViewVolunteers(int postId)
        {

            var volunteers = Context.UserPostRelation
                .Include(upr => upr.User)
                .Where(upr => upr.PostId == postId)
                .Select(upr => new VolunteerInfo
                {
                    FirstName = upr.User.FirstName,
                    LastName = upr.User.LastName
                })
                .ToArray();
            return volunteers;
        }


        /// <summary>
        /// Find and removes all posts in the database that have expired.
        /// That is, their last date is older than the current day.
        /// </summary>
        public void DeleteExpiredPosts()
        {
            // find all expired posts
            var expiredPosts = from posts in Context.Recruitments
                        where posts.LastDate < DateTime.Today
                        select posts;

            // remove expired posts
            foreach (var post in expiredPosts)
            {
                RemoveIfExists(Context.Recruitments, post.Id);
            }
        }

        /// <summary>
        /// Combines conditions into a single condition for a .Where() method.
        /// (all conditions have the same parameter)
        /// </summary>
        /// <typeparam name="T">parameter type for the conditions</typeparam>
        /// <param name="conditions">list of conditions to sum.</param>
        /// <returns>the cumulative condition</returns>
        public static Expression<Func<T, bool>> CombineConditions<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            // Sanity check: given conditions must not be emptyu
            if (!conditions.Any())
            {
                // tautology predicate (always returns true)
                return _ => true;
            }

            // Accumulates the conditions into a single expression
            var finalCondition = conditions
                .Aggregate((current, next) =>
                {
                    // Combine the current and next expressions
                    var parameter = current.Parameters[0];
                    var combinedBody = Expression.AndAlso(
                        current.Body,
                        Expression.Invoke(next, parameter)
                    );
                    return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
                });

            return finalCondition;
        }


        /// <summary>
        /// Adds an entity to the database if and only if it does not already exist, 
        /// where "exists" means that there is no entity in the given dbSet with identical key(s) to the given entity.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="dbSet">the table to add to</param>
        /// <param name="entity">the entity to add</param>
        /// <returns>The entry to this entity, or null if the entry exists already.</returns>
        private EntityEntry<T>? AddIfNotExists<T>(DbSet<T> dbSet, T entity) where T : class
        {
            var keyProperties = Context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties;

            var keyValues = keyProperties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray();

            var existingEntity = dbSet.Find(keyValues);

            if (existingEntity == null)
            {
                var result = dbSet.Add(entity);
                Context.SaveChanges();
                return result;
            }

            return null;
        }
        /// <summary>
        /// Removes an entity based on a given primary key.
        /// NOTICE: Throws exception if key type of set mismatches given key type 'K'.
        /// </summary>
        /// <typeparam name="T">type of entity.</typeparam>
        /// <typeparam name="K">type of key of the entity to remove.</typeparam>
        /// <param name="set">table the entity is in</param>
        /// <param name="key">the primary key of the entity</param>
        /// <returns>true if the entity was found and removed successfully, otherwise false</returns>
        private bool RemoveIfExists<T, K>(DbSet<T> set, K key) where T : class, new()
        {
            // Find the entity by key
            var entity = set.Find(key);

            if (entity == null)
            {
                return false; // Entity does not exist
            }

            set.Remove(entity);
            Context.SaveChanges();
            return true; // Entity existed and was removed
        }

        /// <summary>
        /// Removes an entity based on a given COMPOSITE primary key.
        /// </summary>
        /// <typeparam name="T">type of entity.</typeparam>
        /// <param name="set">table the entity is in</param>
        /// <param name="keyValues">the primary key of the entity</param>
        /// <returns>true if the entity was found and removed successfully, otherwise false</returns>
        private bool RemoveIfExists<T>(DbSet<T> set, params object[] keyValues) where T : class, new()
        {
            // Find the entity by composite keys
            var entity = set.Find(keyValues);

            if (entity == null)
            {
                return false; // Entity does not exist
            }

            set.Remove(entity);
            Context.SaveChanges();
            return true; // Entity existed and was removed
        }

        /// <summary>
        /// Info about the post that is returned
        /// </summary>
        public class PostInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Address { get; set; }
            public Location Location { get; set; }
            public Job Job { get; set; }
            public DateTime InitialDate { get; set; }
            public DateTime LastDate { get; set; }
            public string PhoneNumber { get; set; }

            public bool IsUserInPost {  get; set; }
        }

        /// <summary>
        /// info about volunteers that is returned to client
        /// </summary>
        public class VolunteerInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }

}
