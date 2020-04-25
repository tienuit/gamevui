using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameVui.Server.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public string Avatar { get; set; }
        public virtual ICollection<Match> HostMatches { get; set; }
        public virtual ICollection<Match> GuestMatches { get; set; }

        public virtual ICollection<Match> WinMatches { get; set; }
       

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

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        
        public DbSet<Match> Matches { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Match>()
                .HasRequired(m => m.Player1)
                .WithMany(p => p.HostMatches)
                .HasForeignKey(m => m.PlayerId1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Match>()
                .HasRequired(m => m.Player2)
                .WithMany(p => p.GuestMatches)
                .HasForeignKey(m => m.PlayerId2)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Match>()
                .HasOptional(m => m.Winner)
                .WithMany(p => p.WinMatches)
                .HasForeignKey(m => m.WinnerId)
                .WillCascadeOnDelete(false);
        }
    }
}