using AutoMapper;
using Chinook.ClientModels;
using Chinook.Utils;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<List<Artist>> GetArtistsAsync()
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var artists = await dbContext.Artists
                .Include(a => a.Albums)
                .AsNoTracking()
                .OrderBy(t => t.ArtistId)
                .ToListAsync()
                .ConfigureAwait(false);

            return _mapper.Map<List<Artist>>(artists);
        }

        public async Task<Artist> GetArtistAsync(long artistId)
        {
            var artists = await GetArtistsAsync().ConfigureAwait(false);
            return artists.Where(a => a.ArtistId == artistId).FirstOrDefault();
        }

        public async Task<List<Artist>> SearchArtistsAsync(string query)
        {
            var artists = await GetArtistsAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(query))
            {
                return artists
                    .Where(a => a.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return artists;
        }

        public async Task<List<PlaylistTrack>> GetTracksForArtistAsync(long artistId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID is required for this operation.");
            }
            await using var dbContext = _dbFactory.CreateDbContext();
            var tracks = await dbContext.Tracks
                .Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Include(p => p.Playlists).ThenInclude(u => u.UserPlaylists)
                .AsNoTracking().Select(t => CustomMapper.MapToViewModel(t, userId))
                .ToListAsync()
                .ConfigureAwait(false);

            return tracks;
        }

        public async Task<List<Album>> GetAlbumsForArtistAsync(int artistId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var albums = await dbContext.Albums
                .Where(a => a.ArtistId == artistId)
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .ToListAsync()
                .ConfigureAwait(false);

            return _mapper.Map<List<Album>>(albums);
        }
    }
}
