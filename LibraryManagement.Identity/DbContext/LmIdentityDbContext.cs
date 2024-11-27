using LibraryManagement.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Identity.DbContext
{
    public class LmIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public LmIdentityDbContext(
            DbContextOptions<LmIdentityDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(LmIdentityDbContext).Assembly);
            base.OnModelCreating(builder);

        }
    }
}
