using System.ComponentModel.DataAnnotations.Schema;

namespace LAB1.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public int MangaId { get; set; }
        public Manga Manga { get; set; }
        public int VolumeNumber { get; set; } 
        public int ChapterNumber { get; set; } 
        public List<Page> Pages { get; set; } = new List<Page>();
        public DateTime UpdatedAt { get; set; } = DateTime.Now; 
        [NotMapped] public int CurrentPageNumber { get; set; }
    }
}
