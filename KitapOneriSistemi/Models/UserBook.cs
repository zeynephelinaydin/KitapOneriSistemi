using KitapOneriSistemi.Enums;

namespace KitapOneriSistemi.Models
{
    public class UserBook
    {
        public int UserId { get; set; }
        public string ISBN { get; set; }

        // Enum tipi olarak status kullanılıyor
        public KitapDurumu Status { get; set; } // KitapDurumu enum

        public DateTime AddedDate { get; set; } // Varsayılan CURRENT_TIMESTAMP
        public DateTime? FinishedDate { get; set; } // Nullable çünkü bitiş tarihi her zaman olmayabilir

        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }
    }
}