using System.Linq;
using System.Reflection;
using Game.Application.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Game.Application.Infra.DataAccess
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=game.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "Game.Application.Domain.Entity" && t != typeof(BaseEntity));

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}