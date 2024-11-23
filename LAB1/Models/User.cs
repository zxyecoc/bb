using Microsoft.AspNetCore.Identity;

namespace LAB1.Models
{
    public class User: IdentityUser
    {
        public List<UserFavoriteTeam> UserFavoriteTeams { get; set; } = new List<UserFavoriteTeam>();
    }
}
