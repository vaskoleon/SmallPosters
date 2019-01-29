using Microsoft.EntityFrameworkCore;
using SmallPosters.Models;
using System;

namespace SmallPosters.Data
{
    public class SmallPostersContext:DbContext
    {
        public SmallPostersContext(DbContextOptions<SmallPostersContext> dbContextOptions)
            :base(dbContextOptions)
        {
            
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
    }
}
