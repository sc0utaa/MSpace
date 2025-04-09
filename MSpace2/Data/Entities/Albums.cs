namespace MSpace2.Data.Entities
{
    public class Albums
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
    }
}
