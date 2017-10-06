using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using CORE.IS.Authorization.Roles;
using CORE.IS.Authorization.Users;
using CORE.IS.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace CORE.IS.EntityFrameworkCore
{
    public class ISDbContext : AbpZeroDbContext<Tenant, Role, User, ISDbContext>, IAbpPersistedGrantDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
        
        public ISDbContext(DbContextOptions<ISDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
