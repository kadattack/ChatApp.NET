using ChatApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data;

// public class DataContext : DbContext
public class DataContext : IdentityDbContext<AppUsers,AppRole,int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>

{
    public DataContext(DbContextOptions options) : base(options)
    {  }

    // public DbSet<AppUsers> Users { get; set; }
    public DbSet<AppTopics> Topics { get; set; }
    public DbSet<AppRooms> Rooms { get; set; }
    public DbSet<AppMessages> Messages { get; set; }
    public DbSet<AppImageObject> ImageObjects { get; set; }

    // public DbSet<AppRoomsAppUsers> AppRoomsAppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppRooms>()
            .HasOne<AppUsers>(x=>x.Host)
            .WithMany(g => g.HostOfRooms)
            .HasForeignKey(s => s.HostId);


    }
}