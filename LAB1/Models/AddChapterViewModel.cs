namespace LAB1.Models
{
    public class AddChapterViewModel
    {
        public int MangaId { get; set; }
        public int ChapterNumber { get; set; }  // Номер розділу
        public int VolumeNumber { get; set; }   // Номер тому
        public List<string> PageImageUrls { get; set; }  // Список URL зображень для сторінок розділу
    }

}
