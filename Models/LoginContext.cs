using Microsoft.EntityFrameworkCore;

namespace LoginAndReg.Models
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get; set;}
    }
}