namespace LAB1.Models
{
    public class Team
    {
        public int Id { get; set; } // Унікальний ідентифікатор команди
        public string Name { get; set; } // Назва команди

        // Зв'язок багато до багатьох
        public ICollection<LeagueTeam> LeagueTeams { get; set; } = new List<LeagueTeam>();
        public ICollection<UserFavoriteTeam> UserFavoriteTeams { get; set; } = new List<UserFavoriteTeam>();
    }
}
