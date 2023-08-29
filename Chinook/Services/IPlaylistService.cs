using Chinook.ClientModels;

namespace Chinook.Services
{
    /// <summary>
    /// Represents a service for managing playlists and related operations.
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        /// Adds a track to the user's list of favorite tracks.
        /// </summary>
        /// <param name="trackId">The ID of the track to be added.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddFavoriteTrackAsync(string userId, long trackId);

        /// <summary>
        /// Adds a track to the user's list of favorite tracks.
        /// </summary>
        /// <param name="trackId">The ID of the track to be added.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UnFavoriteTrackAsync(string userId, long trackId);

        /// <summary>
        /// Checks if a user has a favorite playlist.
        /// </summary>
        Task<bool> IsUserHasFavoritePlaylistAsync(string userId);

        /// <summary>
        /// Adds track to a playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <param name="trackId">Track Id to be added to the playlist.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddTracksToPlaylistAsync(long playlistId, long trackId);

        /// <summary>
        /// Removes a track from a playlist.
        /// </summary>
        Task RemoveTrackFromPlaylistAsync(long trackId, long playlistId);

        /// <summary>
        /// Retrieves a list of all playlists.
        /// </summary>
        Task<List<PlayList>> GetPlaylistsAsync();

        /// <summary>
        /// Retrieves a list of all playlists of given user.
        /// </summary>
        Task<List<PlayList>> GetPlaylistsAsync(string userId);

        /// <summary>
        /// Retrieves a playlist with its tracks and favorites for the specified playlist and user.
        /// </summary>
        Task<PlayList> GetPlaylistAsync(string userId, long playlistId);

        /// <summary>
        /// Creates a new playlist with the specified name for the user.
        /// </summary>
        Task<PlayList> CreatePlaylistAsync(string playlistName, string userId);
    }
}
