using Chinook.ClientModels;

namespace Chinook.Services
{
    /// <summary>
    /// Represents a service for managing artist-related operations.
    /// </summary>
    public interface IArtistService
    {
        /// <summary>
        /// Retrieves a list of all artists.
        /// </summary>
        Task<List<Artist>> GetArtistsAsync();

        /// <summary>
        /// Retrieves artist information for the specified artist.
        /// </summary>
        Task<Artist> GetArtistAsync(long artistId);

        /// <summary>
        /// Searches for artists based on a query.
        /// </summary>
        Task<List<Artist>> SearchArtistsAsync(string query);

        /// <summary>
        /// Retrieves a list of playlist tracks for the specified artist and user.
        /// </summary>
        /// <param name="artistId">The ID of the artist.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of playlist tracks associated with the artist.</returns>
        Task<List<PlaylistTrack>> GetTracksForArtistAsync(long artistId, string userId);

        /// <summary>
        /// Retrieves a list of albums for the specified artist.
        /// </summary>
        Task<List<Album>> GetAlbumsForArtistAsync(int artistId);
    }
}
