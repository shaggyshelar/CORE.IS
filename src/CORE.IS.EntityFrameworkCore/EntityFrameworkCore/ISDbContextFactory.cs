using CORE.IS.Configuration;
using CORE.IS.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CORE.IS.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ISDbContextFactory : IDesignTimeDbContextFactory<ISDbContext>
    {
        public ISDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ISDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ISDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ISConsts.ConnectionStringName));

            return new ISDbContext(builder.Options);
        }
    }
}