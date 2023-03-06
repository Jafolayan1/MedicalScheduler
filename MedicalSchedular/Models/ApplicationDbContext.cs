using MedicalSchedular.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalScheduler.Models
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            SeedUsers(builder);
            //SeedPatients(builder);
            base.OnModelCreating(builder);
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Appointment> Appointments { get; set; }


        private static void SeedUsers(ModelBuilder builder)
        {
            User admin = new()
            {
                Id = 1,
                FirstName = " Super",
                LatsName = "Admin",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "1234567890",
                LockoutEnabled = false,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            PasswordHasher<User> passwordHasher = new();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "$Admin1");

            builder.Entity<User>().HasData(admin);
        }

        private static void SeedPatients(ModelBuilder builder)
        {
            builder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    FullName = "AFOLAYAN OLUWATOSIN J.",
                    MatricNo = "ND20200104529",
                    Email = "jafolayan06@gmail.com",
                    Contact = "09090087444",
                    Level = "ND",
                    Department = "Computer Science",
                    SchoolFee = "Paid"
                },
                new Student
                {
                    StudentId = 2,
                    FullName = "ADEOTI ELIJAH D.",
                    MatricNo = "HC20200100461",
                    Email = "jafolayan06@gmail.com",
                    Contact = "09090087444",
                    Level = "HND",
                    Department = "Computer Science",
                    SchoolFee = "Not_Paid"
                },
                new Student
                {
                    StudentId = 3,
                    FullName = "OJO TIMOTHY",
                    MatricNo = "ND20200103985",
                    Email = "jafolayan06@gmail.com",
                    Contact = "09090087444",
                    Level = "ND",
                    Department = "Computer Science",
                    SchoolFee = "Paid"
                },
                new Student
                {
                    StudentId = 4,
                    FullName = "HUSSAIN HADEEZAH B.",
                    MatricNo = "HC0200101104",
                    Email = "jafolayan06@gmail.com",
                    Contact = "09090087444",
                    Level = "HND",
                    Department = "Computer Science",
                    SchoolFee = "Not_Paid"
                },
                 new Student
                 {
                     StudentId = 5,
                     FullName = "HUSSAIN BAYONLE",
                     MatricNo = "HC2020010600",
                     Email = "jafolayan06@gmail.com",
                     Contact = "09090087444",
                     Level = "HND",
                     Department = "Computer Science",
                     SchoolFee = "Not_Paid"
                 });
        }
    }

}
