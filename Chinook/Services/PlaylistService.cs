using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Utils;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly ISharedService _sharedService;
        private readonly IMapper _mapper;

        public PlaylistService(IDbContextFactory<ChinookContext> dbFactory, ISharedService sharedService, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _sharedService = sharedService;
            _mapper = mapper;
        }

        public async Task AddFavoriteTrackAsync(string userId, long trackId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("userId is required for this operation.");
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();

            var favoritePlaylist = await dbContext.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.UserPlaylists.Any(up => up.UserId == userId
                 && p.Name == Constants.FavoriteTracksPlayListName));

            if (favoritePlaylist == null)
            {
                favoritePlaylist = await CreateFavoritePlaylistAsync(userId);
            }
            var track = dbContext.Tracks.Find(trackId);
            if (track != null)
            {
                if (favoritePlaylist.Tracks.Contains(track))
                {
                    throw new InvalidOperationException($"track Id {trackId} already added to favourite list.");
                }
                favoritePlaylist.Tracks.Add(track);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidDataException("Valid track Id not found.");
            }
        }

        public async Task UnFavoriteTrackAsync(string userId, long trackId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("userId is required for this operation.");
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();

            var favoritePlaylist = await dbContext.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.UserPlaylists.Any(up => up.UserId == userId
                                            && p.Name == Constants.FavoriteTracksPlayListName));

            if (favoritePlaylist == null)
            {
                throw new InvalidOperationException("Valid favorite playlist is required for this operation.");
            }

            var track = favoritePlaylist.Tracks.FirstOrDefault(t => t.TrackId == trackId);

            if (track != null)
            {
                favoritePlaylist.Tracks.Remove(track);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidDataException("Valid track Id not found.");
            }
        }

        public async Task<PlayList> GetPlaylistAsync(string userId, long playlistId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("userId is required for this operation.");
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var favouriteTrackIds = await dbContext.Playlists
                        .Where(p => p.UserPlaylists.Any(up => up.UserId == userId) &&
                                p.Name == Constants.FavoriteTracksPlayListName)
                        .AsNoTracking()
                        .SelectMany(p => p.Tracks.Select(t => t.TrackId))
                        .ToListAsync();

            var playlists = await dbContext.Playlists
                          .Include(p => p.Tracks).ThenInclude(t => t.Album).ThenInclude(g => g.Artist)
                          .Where(p => p.PlaylistId == playlistId)
                          .Select(p => CustomMapper.MapToViewModel(p, favouriteTrackIds))
                          .AsNoTracking()
                          .FirstOrDefaultAsync();

            return playlists;
        }

        public async Task<bool> IsUserHasFavoritePlaylistAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("userId is required for this operation.");
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();

            var userHasFavoritePlaylist = await dbContext.Playlists
                .Include(u => u.UserPlaylists)
                .Where(p => p.UserPlaylists.Any(up => up.UserId == userId
                                && up.Playlist.Name == Constants.FavoriteTracksPlayListName))
                .AnyAsync();

            return userHasFavoritePlaylist;
        }

        public async Task AddTracksToPlaylistAsync(long playlistId, long trackId)
        {
            if (playlistId < 1)
            {
                throw new InvalidOperationException("Invalid playlist Id.");
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var playlist = await dbContext.Playlists.Include(p => p.Tracks)
                                            .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
            if (playlist != null)
            {
                var trackToAdd = await dbContext.Tracks.Where(t => t.TrackId == trackId).FirstAsync();

                if (!playlist.Tracks.Any(pt => pt.TrackId == trackToAdd.TrackId))
                {
                    playlist.Tracks.Add(trackToAdd);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException($"{trackToAdd.Name} Already added to playlist {playlist.Name}.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Playlist id {playlistId} not found.");
            }
        }

        public async Task<List<PlayList>> GetPlaylistsAsync()
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var playlists = await dbContext.Playlists.Include(x => x.UserPlaylists)
                          .OrderByDescending(p => p.Name == Constants.FavoriteTracksPlayListName)
                          .ThenBy(p => p.PlaylistId)
                          .ToListAsync();
            return _mapper.Map<List<PlayList>>(playlists);
        }

        public async Task<List<PlayList>> GetPlaylistsAsync(string userId)
        {
            var playlists = await GetPlaylistsAsync().ConfigureAwait(false);
            return playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId)).ToList();
        }

        public async Task<PlayList> CreatePlaylistAsync(string playlistName, string userId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = await dbContext.Users
                .Include(u => u.UserPlaylists)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var existingPlaylist = user.UserPlaylists.FirstOrDefault(p => p.Playlist?.Name == playlistName);
            if (existingPlaylist != null)
            {
                throw new InvalidOperationException("Playlist with the same name already exists.");
            }

            var newPlayList = new Playlist()
            {
                Name = playlistName,
                PlaylistId = await GetNextPlaylistIdAsync()
            };

            dbContext.Playlists.Add(newPlayList);
            await dbContext.SaveChangesAsync();

            var newUserPlaylist = new Chinook.Models.UserPlaylist
            {
                UserId = userId,
                PlaylistId = newPlayList.PlaylistId
            };

            dbContext.UserPlaylists.Add(newUserPlaylist);
            await dbContext.SaveChangesAsync();

            _sharedService.NotifyPlaylistsChanged();

            return _mapper.Map<PlayList>(newPlayList);
        }

        public async Task RemoveTrackFromPlaylistAsync(long trackId, long playlistId)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var playlist = await dbContext.Playlists.Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist != null)
            {
                var trackToRemove = playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);

                if (trackToRemove != null)
                {
                    playlist.Tracks.Remove(trackToRemove);
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                throw new InvalidOperationException("Playlist not found.");
            }
        }

        private async Task<Playlist> CreateFavoritePlaylistAsync(string userId)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();

            var favoritePlaylist = new Playlist
            {
                Name = Constants.FavoriteTracksPlayListName,
                PlaylistId = await GetNextPlaylistIdAsync(),
                UserPlaylists = new List<Chinook.Models.UserPlaylist>
                {
                    new Chinook.Models.UserPlaylist { UserId = userId }
                }
            };

            dbContext.Playlists.Add(favoritePlaylist);
            await dbContext.SaveChangesAsync();
            _sharedService.NotifyPlaylistsChanged();

            return favoritePlaylist;
        }

        //temp method due to db error.
        //need to add auto increment to PlaylistId column of playlist table.
        //Can't add increment constraint to mention column in sqlite.
        //after add it, remove this method.
        private async Task<long> GetNextPlaylistIdAsync()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var playlists = await dbContext.Playlists.ToListAsync();
            return playlists.Max(x => x.PlaylistId) + 1;
        }
    }
}
