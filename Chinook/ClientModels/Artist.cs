namespace Chinook.ClientModels
{
    public class Artist
    {
        public required long ArtistId { get; set; }
        public string Name { get; set; }

        public List<Album> Albums { get; set; }
    }
}
