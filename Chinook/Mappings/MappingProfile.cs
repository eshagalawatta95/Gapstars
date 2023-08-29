using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //db models to client models
            CreateMap<Playlist, PlayList>()
                .ForMember(d => d.UserPlaylists, opt => opt.MapFrom(s => s.UserPlaylists));
            CreateMap<Chinook.Models.Album, Chinook.ClientModels.Album>();
            CreateMap<Chinook.Models.Artist, Chinook.ClientModels.Artist>();
            CreateMap<Track, PlaylistTrack>();
            CreateMap<Chinook.Models.UserPlaylist, Chinook.ClientModels.UserPlaylist>();
        }
    }
}
