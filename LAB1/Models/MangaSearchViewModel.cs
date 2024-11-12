namespace LAB1.Models
{
    public class MangaSearchViewModel
    {
        public string SearchQuery { get; set; }  // Поле для введення пошукового запиту
        public List<Manga> Results { get; set; } // Результати пошуку
    }
}
