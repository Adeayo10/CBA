using CBA.Models;
using CBA.Models.TokenModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CBA.Context
{
    public class UserDataContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<BankBranch> BankBranch { get; set; }

        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options)
        {
        }

     
    }
}
