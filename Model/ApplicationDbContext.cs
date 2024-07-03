using BackEnd.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext ()
    {
    }

    public ApplicationDbContext ( DbContextOptions<ApplicationDbContext> options )
        : base(options)
    { }

    public DbSet<Reservation> reservations { get; set; }
    public DbSet<Member> members { get; set; }
    public DbSet<Fine> fines { get; set; }
    public DbSet<FineDate>  fineDates { get; set; }
    public DbSet<Borrow> borrows { get; set; }
    public DbSet<Book> books { get; set; }



}
