using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSpace2.Data;
using MSpace2.Data.Entities;
using MSpace2.Data.Migrations;
using MSpace2.Services;

namespace MSpace2.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAlbumRatingService _ratingService;

        public AlbumsController(ApplicationDbContext context, IAlbumRatingService ratingService)
        {
            _context = context;
            _ratingService= ratingService;
        }
        [Authorize]
        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums.ToListAsync();

            // Create a dictionary to store album ratings
            var albumRatings = new Dictionary<int, (double? AverageRating, int Count)>();

            // Get ratings for all albums
            foreach (var album in albums)
            {
                var averageRating = await _ratingService.GetAverageRatingAsync(album.Id);
                var ratingCount = await _ratingService.GetRatingCountAsync(album.Id);
                albumRatings[album.Id] = (averageRating, ratingCount);
            }

            // Pass the ratings to the view via ViewBag
            ViewBag.AlbumRatings = albumRatings;

            return View(albums);
        }
        [Authorize]
        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albums == null)
            {
                return NotFound();
            }

            var averageRating = await _ratingService.GetAverageRatingAsync(albums.Id);
            var ratingCount = await _ratingService.GetRatingCountAsync(albums.Id);

            // Get current user's rating if logged in
            int? userRating = null;
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userRating = await _ratingService.GetUserRatingAsync(albums.Id, userId);
            }

            ViewBag.AverageRating = averageRating;
            ViewBag.RatingCount = ratingCount;
            ViewBag.UserRating = userRating;

            return View(albums);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rate(int albumId, int rating)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Save the rating
                bool success = await _ratingService.RateAlbumAsync(albumId, userId, rating);

                if (success)
                {
                    // Get updated rating info
                    var averageRating = await _ratingService.GetAverageRatingAsync(albumId);
                    var ratingCount = await _ratingService.GetRatingCountAsync(albumId);

                    return Json(new
                    {
                        success = true,
                        averageRating = averageRating ?? 0, // Handle null by providing default value
                        ratingCount = ratingCount
                    });
                }
            }

            return Json(new { success = false });
        }
        [Authorize(Roles = "Admin")]

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ArtistName,ReleaseDate,Description,CoverImageUrl")] MSpace2.Data.Entities.Albums albums)
        {
            if (ModelState.IsValid)
            {
                _context.Add(albums);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(albums);
        }
        [Authorize(Roles = "Admin")]
        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums.FindAsync(id);
            if (albums == null)
            {
                return NotFound();
            }
            return View(albums);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ArtistName,ReleaseDate,Description,CoverImageUrl")] MSpace2.Data.Entities.Albums albums)
        {
            if (id != albums.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albums);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumsExists(albums.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(albums);
        }

        // GET: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (albums == null)
            {
                return NotFound();
            }

            return View(albums);
        }

        // POST: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var albums = await _context.Albums.FindAsync(id);
            if (albums != null)
            {
                _context.Albums.Remove(albums);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumsExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}