namespace Chinook.ClientModels
{
    public class PlayList
    {
        public long PlaylistId { get; set; }
        public string Name { get; set; }

        public List<PlaylistTrack> Tracks { get; set; }
        public List<UserPlaylist> UserPlaylists { get; set; }
    }
}
