namespace LAB1.Models
{
    public class UserFavoriteTeam
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
