namespace LAB1.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Team> FavoriteTeams { get; set; } // Список улюблених команд

    }
}
