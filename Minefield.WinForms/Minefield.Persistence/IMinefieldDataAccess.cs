namespace Minefield.Persistence
{
    public interface IMinefieldDataAccess
    {
        Task<MinefieldGrid> LoadAsync(Stream stream);
        Task SaveAsync(String path, MinefieldGrid table);
    }
}
