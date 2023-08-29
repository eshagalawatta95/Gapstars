namespace Chinook.ClientModels
{
    public class UserPlaylist
    {
            public required string UserId { get; set; }
            public required long PlaylistId { get; set; }
            public PlayList Playlist { get; set; }
        }

}
