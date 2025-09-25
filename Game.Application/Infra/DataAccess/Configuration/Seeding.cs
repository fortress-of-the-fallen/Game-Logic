using System;
using Game.Application.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Game.Application.Infra.DataAccess.Configuration
{
    public static class Seeding
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            SeedSettings(modelBuilder);
        }

        private static void SeedSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(
                new Setting { Id = Guid.Parse("e1026eeb-f868-40c3-b65b-b627c289c6ad"), Key = "Language", Value = "en" }
            );
        }
    }
}