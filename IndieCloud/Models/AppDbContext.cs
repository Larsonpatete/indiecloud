using Microsoft.EntityFrameworkCore;

namespace IndieCloud.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages { get; set; }
    public DbSet<StreamObject> StreamObjects { get; set; }
}
