namespace LAB1.Models
{
    public class LeagueTeam
    {
        public int LeagueId { get; set; } // Ідентифікатор ліги
        public League League { get; set; } // Навігаційна властивість

        public int TeamId { get; set; } // Ідентифікатор команди
        public Team Team { get; set; } // Навігаційна властивість
    }
}
