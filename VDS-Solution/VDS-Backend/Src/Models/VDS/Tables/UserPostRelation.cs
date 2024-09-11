using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDS_Backend.Src.Models.VDS.Tables
{
    // NOTE TO SELF: EntityFramework automatically implemented delete cascade.
    // That is, if either User gets deleted or Post gets deleted, so will any relations they participate in.

    /// <summary>
    /// A many-to-many relationship between users and posts.
    /// The relationship represents users joining the volunteer team of the post.
    /// </summary>
    [PrimaryKey(nameof(UserEmail), nameof(PostId))]
    internal class UserPostRelation
    {

        [ForeignKey("User")]
        public string UserEmail { get; set; }
        [ForeignKey("RecruitmentPost")]
        public int PostId { get; set; }

        // user navigation property
        public User User { get; set; }
        // post navigation property
        public RecruitmentPost Post { get; set; }
    }
}
