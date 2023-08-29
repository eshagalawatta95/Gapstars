namespace Chinook.ClientModels
{
    public class Album
    {
        public required long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public required long ArtistId { get; set; }

        public Artist Artist { get; set; } = null!;
        public  List<PlaylistTrack> Tracks { get; set; }
    }
}
