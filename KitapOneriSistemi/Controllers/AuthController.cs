using KitapOneriSistemi.DTOs;
using KitapOneriSistemi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KitapOneriSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Kullanıcı kaydı
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var isRegistered = await _authService.RegisterUser(registrationDto.Username, registrationDto.Email, registrationDto.Password);
                if (!isRegistered)
                {
                    return BadRequest("Kullanıcı adı ya da email zaten mevcut.");
                }
                return Ok("Kullanıcı başarıyla kaydedildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Hata mesajını döndür
            }
        }

        // Kullanıcı girişi
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var isAuthenticated = await _authService.LoginUser(loginDto.Email, loginDto.Password);
                if (isAuthenticated)
                {
                    return Ok("Giriş başarılı");
                }
                return Unauthorized("Geçersiz giriş bilgileri");
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message); // Hata mesajını döndür
            }
        }
    }
}