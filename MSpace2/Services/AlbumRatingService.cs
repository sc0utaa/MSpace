using MSpace2.Data.Entities;
using MSpace2.Data;
using Microsoft.EntityFrameworkCore;

namespace MSpace2.Services
{
    public class AlbumRatingService : IAlbumRatingService
    {
        private readonly ApplicationDbContext _context;

        public AlbumRatingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RateAlbumAsync(int albumId, string userId, int rating)
        {
            // Check if user already rated this album
            var existingRating = await _context.AlbumRatings
                .FirstOrDefaultAsync(r => r.AlbumId == albumId && r.UserId == userId);

            if (existingRating != null)
            {
                // Update existing rating
                existingRating.Rating = rating;
            }
            else
            {
                // Create new rating
                _context.AlbumRatings.Add(new AlbumRating
                {
                    AlbumId = albumId,
                    UserId = userId,
                    Rating = rating
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double?> GetAverageRatingAsync(int albumId)
        {
            return await _context.AlbumRatings
                .Where(r => r.AlbumId == albumId)
                .AverageAsync(r => (double?)r.Rating);
        }

        public async Task<int?> GetUserRatingAsync(int albumId, string userId)
        {
            var rating = await _context.AlbumRatings
                .FirstOrDefaultAsync(r => r.AlbumId == albumId && r.UserId == userId);

            return rating?.Rating;
        }

        public async Task<int> GetRatingCountAsync(int albumId)
        {
            return await _context.AlbumRatings
                .CountAsync(r => r.AlbumId == albumId);
        }
    }
}