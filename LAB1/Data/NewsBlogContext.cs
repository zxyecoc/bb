using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LAB1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LAB1.Data
{
    public class NewsBlogContext : IdentityDbContext<User>
    {
        public NewsBlogContext(DbContextOptions<NewsBlogContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; } = default!;

        // Зміна назви Author на Authors
        public DbSet<Author> Authors { get; set; } = default!;
        // Зміна назви Tag на Tags
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Likes> Ratings { get; set; } = default!;
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<LeagueTeam> LeagueTeams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв'язку для автора
            modelBuilder.Entity<News>()
                .HasOne(m => m.Author)
                .WithMany()
                .HasForeignKey(m => m.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); // Використовуємо каскад для автора

            modelBuilder.Entity<News>()
            .HasMany(m => m.Tags)
            .WithMany(t => t.News)
            .UsingEntity(j => j.ToTable("NewsTags")); // Створення зв'язуючої таблиці

            // Налаштування зв'язку багато до багатьох
            modelBuilder.Entity<LeagueTeam>()
                .HasKey(lt => new { lt.LeagueId, lt.TeamId }); // Composite primary key

            modelBuilder.Entity<LeagueTeam>()
                .HasOne(lt => lt.League)
                .WithMany(l => l.LeagueTeams)
                .HasForeignKey(lt => lt.LeagueId);

            modelBuilder.Entity<LeagueTeam>()
                .HasOne(lt => lt.Team)
                .WithMany(t => t.LeagueTeams)
                .HasForeignKey(lt => lt.TeamId);
        }

    }
    
}
