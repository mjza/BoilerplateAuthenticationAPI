using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Entities.Auth;

namespace WebApi.Helpers.Auth
{
    public class AccountDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        private readonly IConfiguration Configuration;

        public AccountDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}