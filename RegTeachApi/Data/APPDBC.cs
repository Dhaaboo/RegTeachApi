using Microsoft.EntityFrameworkCore;
using RegTeachApi.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace RegTeachApi.Data
{
    public class APPDBC : DbContext
    {
        public APPDBC(DbContextOptions<APPDBC> options) : base(options)
        {
        }

        [AllowNull]
        public DbSet<Student> _Students { get; set; }
        public DbSet<RegTeachUsers> RegTeachUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder _builder)
        {
            base.OnModelCreating(_builder);

            // Seed Person Table
            _builder.Entity<Student>().HasData(new Student
            {
                Id = 1,
                Name = "Abdinoor Suleman",
                Email = "abdinoor@example.com"
            });

            _builder.Entity<Student>().HasData(new Student
            {
                Id = 2,
                Name = "Abdirahman Suleman",
                Email = "abdirahman@example.com"

            });

            _builder.Entity<Student>().HasData(new Student
            {
                Id = 3,
                Name = "Abdiqani Suleman",
                Email = "abdiqani@example.com"

            });

        }

    }
}
