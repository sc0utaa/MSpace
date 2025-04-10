namespace MSpace2.Services
{
    public interface IAlbumRatingService
    {
        Task<bool> RateAlbumAsync(int albumId, string userId, int rating);
        Task<double?> GetAverageRatingAsync(int albumId);
        Task<int?> GetUserRatingAsync(int albumId, string userId);
        Task<int> GetRatingCountAsync(int albumId);
    }
}