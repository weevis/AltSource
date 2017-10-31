using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AltSourceBankAppAPI.Models;
using AltSourceBankAppAPI.Entity;

namespace AltSourceBankAppAPI.Contexts
{
    /* API Context, we want the Identity for registration/login purposes */
    public class ApiContext : IdentityDbContext<UserEntity>
    {

        /// <summary>
        /// Our database context
        /// </summary>
        /// <param name="options">Options so we can use the in memory database type</param>
        public ApiContext(DbContextOptions<ApiContext> options ) : base(options)
        {

        }

        /// <summary>
        /// Our Users database "object"
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Our Transactions database "object"
        /// </summary>
        public DbSet<Transactions> Transactions { get; set; }

        /// <summary>
        /// Use fluent to create some database indexes
        /// </summary>
        /// <param name="builder">we use the builder to add the index</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Users>().HasIndex(u => u.UserName).HasName("Index_UserName");
            builder.Entity<Transactions>().HasIndex(t => t.UserID).HasName("Index_UserID");
        }
    }
}
