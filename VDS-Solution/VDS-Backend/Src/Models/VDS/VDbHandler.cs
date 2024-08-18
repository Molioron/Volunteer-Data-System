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
            return RemoveIfNotExists(Context.Users, email);
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
        /// <returns>true if the post was created successfully, otherwise false.</returns>
        /// <exception cref="ArgumentException">if initial date is after last date.</exception>
        public bool addRecruitmentPost(string userEmail, string title, string description, string address,
            Location location, Job job, DateTime initialDate, DateTime lastDate)
        {
            if (initialDate > lastDate) // sanity check: initial date is not after last date.
            {
                throw new ArgumentException("Specified initialDate is after lastDate!");
            }

            var post = new RecruitmentPost()
            { UserEmail = userEmail, Title = title, Description = description, Address = address, Location = location,
            Job = job, InitialDate = initialDate, LastDate = lastDate};
            return AddIfNotExists(Context.Recruitments, post) is not null;
        }
        /// <summary>
        /// Removes a recruitment post from the database based on it's id primary key.
        /// </summary>
        /// <param name="id">the id of the post.</param>
        /// <returns>true if the post was removed successfully, otherwise false.</returns>
        public bool removeRecruitmentPost(int id)
        {
            return RemoveIfNotExists(Context.Recruitments, id);
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
            Job[] jobTypes, DateTime? initialDate, DateTime? endDate, DateFilterType? dateFilterType)
        {
            if (initialDate > endDate) // sanity check: initial date must not be after end date
            { throw new ArgumentException("initialDate must not be after endDate"); }

            // only posts
            var query = Context.Recruitments.Include(r => r.User).AsQueryable();

            // filter by location
            if (volunteerAreas.Length > 0) // filter only when necessary
            {
                query = query.Where(posts => volunteerAreas.Contains(posts.Location));
            }
            // filter by job type
            if (jobTypes.Length > 0) // filter only when necessary
            {
                query = query.Where(posts => jobTypes.Contains(posts.Job));
            }

            // filter by date only when necessary (all not null)
            if (initialDate is not null && endDate is not null && dateFilterType is not null)
            {
                // filter by date according to dateFilterType
                switch (dateFilterType)
                {
                    case DateFilterType.Contains:
                        query = query.Where(posts => initialDate <= posts.InitialDate &&
                        posts.InitialDate <= posts.LastDate &&
                        posts.LastDate <= endDate);
                        break;

                    case DateFilterType.Intersects:
                        query = query.Where(posts => posts.InitialDate <= posts.LastDate &&
                                                     ((initialDate <= posts.LastDate && posts.LastDate <= endDate) ||
                                                      (initialDate <= posts.InitialDate && posts.InitialDate <= endDate)) ||
                                                      (posts.InitialDate <= initialDate && endDate <= posts.LastDate));
                        break;

                    default:
                        throw new NotImplementedException("Unknown DateFilterType value.");
                }
            }
            // result of the query
            var result = query.Select(posts => new PostInfo
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
            catch (Exception ex) { return false; }
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
        /// </summary>
        /// <typeparam name="T">type of entity.</typeparam>
        /// <typeparam name="K">type of key of the entity to remove.</typeparam>
        /// <param name="dbSet">table the entity is in</param>
        /// <param name="key">the primary key of the entity</param>
        /// <returns>true if the entity was found and removed successfully, otherwise false</returns>
        private bool RemoveIfNotExists<T, K>(DbSet<T> set, K key) where T : class, new()
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
        }
    }

}
