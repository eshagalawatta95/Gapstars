using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Utils
{
    public static class CustomMapper
    {
        public static PlaylistTrack MapToViewModel(Track t, string userId)
        {
            return new PlaylistTrack
            {
                AlbumTitle = t.Album == null ? "-" : t.Album.Title,
                ArtistName = t.Album?.Artist?.Name,
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists
                    .Any(pl => pl.UserPlaylists
                        .Any(up => up.UserId == userId && up.Playlist.Name == Constants.FavoriteTracksPlayListName))
            };
        }

        public static PlayList MapToViewModel(Playlist p, string userId)
        {
            return new PlayList
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new PlaylistTrack
                {
                    AlbumTitle = t.Album == null ? "-" : t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                        .Any(pl => pl.UserPlaylists
                            .Any(up => up.UserId == userId && up.Playlist.Name == Constants.FavoriteTracksPlayListName))
                }).ToList()
            };
        }
    }
}
