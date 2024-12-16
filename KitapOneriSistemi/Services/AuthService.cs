using KitapOneriSistemi.Models;
using KitapOneriSistemi.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Threading.Tasks;

namespace KitapOneriSistemi.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUser(string username, string email, string password)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.Username == username);
            if (existingUser != null)
            {
                return false; // Kullanıcı zaten mevcut
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RegistrationDate = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true; // Başarılı kayıt
        }

        public async Task<bool> LoginUser(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false; // Kullanıcı bulunamadı
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash); // Şifre doğrulama
        }
    }
}