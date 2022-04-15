using Microsoft.EntityFrameworkCore;

namespace ZionApps.Outbox.Tests
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; } = null!;
    }

    public class Person
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, Id:{Id}";
        }
    }
}