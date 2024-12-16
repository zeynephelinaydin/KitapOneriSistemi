using KitapOneriSistemi.Enums;
using KitapOneriSistemi.Models;
using Microsoft.EntityFrameworkCore;
using KitapOneriSistemi.Data;  // ApplicationDbContext'in bulunduğu namespace


namespace KitapOneriSistemi.Services
{
    public class BookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Favorilere kitap ekleme
        public async Task<bool> AddToFavorites(int userId, string isbn)
        {
            var existingFavorite = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.ISBN == isbn && ub.Status == KitapDurumu.Wishlist);

            if (existingFavorite != null)
            {
                return false; // Kitap zaten favorilerde
            }

            var userBook = new UserBook
            {
                UserId = userId,
                ISBN = isbn,
                Status = KitapDurumu.Wishlist,
                AddedDate = DateTime.Now
            };

            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            return true; // Favoriye başarıyla eklendi
        }

        // Favorilerden kitap çıkarma
        public async Task<bool> RemoveFromFavorites(int userId, string isbn)
        {
            var userBook = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.ISBN == isbn && ub.Status == KitapDurumu.Wishlist);

            if (userBook == null)
            {
                return false; // Kitap favorilerde bulunamadı
            }

            _context.UserBooks.Remove(userBook);
            await _context.SaveChangesAsync();

            return true; // Kitap başarıyla favorilerden çıkarıldı
        }

        // Favori kitapları listeleme
        public async Task<List<Book>> GetFavoriteBooks(int userId)
        {
            var favoriteBooks = await _context.UserBooks
                .Where(ub => ub.UserId == userId && ub.Status == KitapDurumu.Wishlist)
                .Include(ub => ub.Book)
                .ToListAsync();

            return favoriteBooks.Select(ub => ub.Book).ToList();
        }
    }

}
