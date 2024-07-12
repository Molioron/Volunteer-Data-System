using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Models.Contexts
{
    /// <summary>
    /// A Facade to the database context to the volunteer system.
    /// facade in a sense that it should not be instanciated and instead a concrete class should
    /// inherit from this class to specify what can of database is created (e.g: sqlite, postregsql, etc...)
    /// </summary>
    internal abstract class VDSContext : DbContext
    {
    }
}
