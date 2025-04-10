using Microsoft.AspNetCore.Identity;
using MSpace2.Data.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSpace2.Data.Entities
{
    public class AlbumRating
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Albums Album { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } // Using the standard IdentityUser

        [Required]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
