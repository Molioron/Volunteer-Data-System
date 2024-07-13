using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VDS_Backend.Src.Models.VDS.Contexts;

namespace VDS_Backend.Src.Models.VDS
{
    /// <summary>
    /// A class responsible for making queries and adding entities to the database.
    /// </summary>
    internal class VDbHandler
    {
        // db context
        private VDSContext Context { get; set; }

        public VDbHandler(VDSContext db) { Context = db; }

        /// <summary>
        /// Adds an entity to the databse if and only if it does not already exist.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="dbSet">the table to add to</param>
        /// <param name="entity">the entity to add</param>
        /// <param name="predicate">additional predicate, an entity exists if it's in the table and satisfies this predicate.
        /// if predicate = null, then the predicate is ignored.</param>
        /// <returns>The entry to this entity</returns>
        public static EntityEntry<T> AddIfNotExists<T>(DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null;
        }
    }

}
