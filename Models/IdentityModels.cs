using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Genealoge.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public int? MyPersonInfoId { get; set; }
        public Person MyPersonInfo { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Person> Persons { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Parents)
                .WithMany(p => p.Children)
                .Map(m => m.ToTable("PeopleParents")
                .MapLeftKey("Person_Id").MapRightKey("Child_Id"));

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Husbands)
                .WithMany(p => p.Wifes)
                .Map(m => m.ToTable("PeopleMarriage")
                .MapLeftKey("Person_Id").MapRightKey("Marriage_Id"));

            modelBuilder.Entity<Person>()
                .HasMany(p => p.ReversedSiblings)
                .WithMany(p => p.Siblings)
                .Map(m => m.ToTable("PeopleSiblings")
                .MapLeftKey("Person_Id").MapRightKey("Sibling_Id"));

            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<Genealoge.Models.PersonShortViewModel> PersonShortViewModels { get; set; }
    }
}