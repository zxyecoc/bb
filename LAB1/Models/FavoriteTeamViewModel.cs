using System.ComponentModel.DataAnnotations;

namespace LAB1.Models
{
    public class FavoriteTeamViewModel
    {
        public string UserId { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>(); // Ініціалізація Teams для уникнення NullReferenceException
        public List<int> SelectedTeamIds { get; set; } = new List<int>(); // Список вибраних команд

    }
}
