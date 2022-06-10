using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TaskA.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Booking> Bookings {get; set;}
        public DbSet<Client> Clients { get; set;}
        public DbSet<Document> documents { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceCat> ServiceCats { get; set; }
        public DbSet<Tasker> Taskers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<OrderStatus> OrderStatuses{ get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Spins> Spins { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<Beneficiary_Signature> beneficiary_Signatures { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //	base.OnModelCreating(modelBuilder);
        //	modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
        public DbSet<IdentityUserRole> UserInRole { get; set; }
        // public DbSet<ApplicationUser> appUsers { get; set; }
        public DbSet<ApplicationRole> appRoles { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

		public System.Data.Entity.DbSet<TaskA.Models.BookingStatus> BookingStatus { get; set; }

		public System.Data.Entity.DbSet<TaskA.Models.QRCode> QRCodes { get; set; }

        public System.Data.Entity.DbSet<TaskA.Models.ToolSet> ToolSets { get; set; }

        public System.Data.Entity.DbSet<TaskA.Models.BookTool> BookTools { get; set; }

        public System.Data.Entity.DbSet<TaskA.Models.Signature> Signatures { get; set; }
        public System.Data.Entity.DbSet<TaskA.Models.Comments> Comments { get; set; }

    }
}