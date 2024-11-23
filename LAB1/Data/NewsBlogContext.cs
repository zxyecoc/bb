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
        public DbSet<Author> Authors { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Likes> Likes { get; set; } = default!; // Змінив назву на Likes
        public DbSet<League> Leagues { get; set; } = default!;
        public DbSet<Team> Teams { get; set; } = default!;
        public DbSet<LeagueTeam> LeagueTeams { get; set; } = default!;
        public DbSet<UserFavoriteTeam> UserFavoriteTeams { get; set; } = default!;

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

            // Налаштування зв'язку багато до багатьох для LeagueTeam
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

            // Налаштування зв'язку для UserFavoriteTeam
            modelBuilder.Entity<UserFavoriteTeam>()
                .HasKey(uf => new { uf.UserId, uf.TeamId });

            modelBuilder.Entity<UserFavoriteTeam>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.UserFavoriteTeams)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадне видалення

            modelBuilder.Entity<UserFavoriteTeam>()
                .HasOne(uf => uf.Team)
                .WithMany(t => t.UserFavoriteTeams)
                .HasForeignKey(uf => uf.TeamId);
        }
    }
    
}
