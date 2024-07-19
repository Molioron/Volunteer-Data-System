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
            RecruitmentPosts = [], VolunteerPosts = []};

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
        public bool addRecruitmentPost(string userEmail, string title, string description, string address,
            Location location, Job job, DateTime initialDate, DateTime lastDate)
        {
            var post = new RecruitmentPost()
            { UserEmail = userEmail, Title = title, Description = description, Address = address, Location = location,
            Job = job, InitialDate = initialDate, LastDate = lastDate};
            return AddIfNotExists(Context.Recruitments, post) is not null;
        }
        //public bool removeRecruitmentPost() {  }
        //public bool addVolunteerPost() { }
        //public bool removeVolunteerPost() { }


        /// <summary>
        /// Adds an entity to the databse if and only if it does not already exist.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="dbSet">the table to add to</param>
        /// <param name="entity">the entity to add</param>
        /// <param name="predicate">additional predicate, an entity exists if it's in the table and satisfies this predicate.
        /// if predicate = null, then the predicate is ignored.</param>
        /// <returns>The entry to this entity, or null if the entry exists already.</returns>
        private EntityEntry<T>? AddIfNotExists<T>(DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            var res = !exists ? dbSet.Add(entity) : null;
            if (res != null) { Context.SaveChanges(); } // save only if there is a change
            return res;
        }
        /// <summary>
        /// Removes an entity based on a given primary key.
        /// </summary>
        /// <typeparam name="T">type of entity.</typeparam>
        /// <param name="dbSet">table the entity is in</param>
        /// <param name="key">the primary key of the entity</param>
        /// <returns>true if the entity was found and removed successfully, otherwise false</returns>
        private bool RemoveIfNotExists<T>(DbSet<T> set, string key) where T : class, new()
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
    }

}
