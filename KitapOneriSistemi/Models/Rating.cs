using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitapOneriSistemi.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public string ISBN { get; set; }

        public int OneStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int FiveStarCount { get; set; }

        [NotMapped]
        public double AverageRating 
        {
            get
            {
                var totalRatings = OneStarCount + TwoStarCount + ThreeStarCount + FourStarCount + FiveStarCount;
                return totalRatings == 0 
                    ? 0 
                    : ((OneStarCount * 1) + (TwoStarCount * 2) + (ThreeStarCount * 3) + (FourStarCount * 4) + (FiveStarCount * 5)) / (double)totalRatings;
            }
        }

        public Book Book { get; set; }
    }
}