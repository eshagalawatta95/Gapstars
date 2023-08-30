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

        public static PlayList MapToViewModel(Playlist playlist, List<long> favouriteTrackIds)
        {
            return new PlayList
            {
                Name = playlist?.Name,
                Tracks = MapTracksWithFavorites(playlist.Tracks, favouriteTrackIds)
            };
        }

        private static List<PlaylistTrack> MapTracksWithFavorites(
                ICollection<Track> tracks, List<long> favouriteTrackIds)
        {
            return tracks.Select(t => new PlaylistTrack
            {
                AlbumTitle = t.Album == null ? "-" : t.Album.Title,
                ArtistName = t.Album?.Artist?.Name,
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = favouriteTrackIds.Contains(t.TrackId)
            }).ToList();
        }
    }
}
