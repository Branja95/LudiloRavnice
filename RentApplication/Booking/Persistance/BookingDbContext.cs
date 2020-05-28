using Booking.Models.Entities;
using Booking.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Persistance
{
    public class BookingDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }
    }
}
