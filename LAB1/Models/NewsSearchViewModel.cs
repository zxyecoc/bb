namespace LAB1.Models
{
    public class NewsSearchViewModel
    {
        public string SearchQuery { get; set; }  // Поле для введення пошукового запиту
        public List<News> Results { get; set; } // Результати пошуку
    }
}
