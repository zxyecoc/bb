namespace LAB1.Models
{
    public class League
    {
        public int Id { get; set; } // Унікальний ідентифікатор ліги
        public string Name { get; set; } // Назва ліги

        // Зв'язок багато до багатьох
        public ICollection<LeagueTeam> LeagueTeams { get; set; } = new List<LeagueTeam>();
    }

}
