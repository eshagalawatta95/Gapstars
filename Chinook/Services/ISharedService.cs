
namespace Chinook.Services
{
    /// <summary>
    /// Represents a service for managing and notifying changes to playlists.
    /// </summary>
    public interface ISharedService
    {
        /// <summary>
        /// Event that is raised when playlists are changed.
        /// Subscribers can register handlers to respond to playlist changes.
        /// </summary>
        event Action PlaylistsChanged;

        /// <summary>
        /// Notifies subscribers that playlists have been changed.
        /// Call this method after adding, modifying, or deleting playlists.
        /// </summary>
        void NotifyPlaylistsChanged();
    }
}
