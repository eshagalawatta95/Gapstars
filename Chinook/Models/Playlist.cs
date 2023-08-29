using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Models
{
    public partial class Playlist
    {
        public Playlist()
        {
            Tracks = new HashSet<Track>();
        }

        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long PlaylistId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
        public virtual ICollection<UserPlaylist> UserPlaylists { get; set; }

    }
}
