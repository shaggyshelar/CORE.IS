using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CORE.IS.EntityFrameworkCore
{
    public static class ISDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ISDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ISDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}