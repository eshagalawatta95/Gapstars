namespace Chinook.Services
{
    public class SharedService: ISharedService
    {
        public event Action PlaylistsChanged;

        public void NotifyPlaylistsChanged()
        {
            PlaylistsChanged?.Invoke();
        }
    }
}
