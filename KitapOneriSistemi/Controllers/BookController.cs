using KitapOneriSistemi.Services;
using Microsoft.AspNetCore.Mvc;
using KitapOneriSistemi.Models;

namespace KitapOneriSistemi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly MenuService _menuService;

        public BookController(BookService bookService, MenuService menuService)
        {
            _bookService = bookService;
            _menuService = menuService;
        }

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("add-to-favorites")]
        public async Task<IActionResult> AddToFavorites([FromQuery] int userId, [FromQuery] string isbn)
        {
            var result = await _bookService.AddToFavorites(userId, isbn);
            if (result)
            {
                return Ok("Kitap favorilere başarıyla eklendi.");
            }
            else
            {
                return BadRequest("Kitap zaten favorilere eklenmiş.");
            }
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoriteBooks([FromQuery] int userId)
        {
            var favoriteBooks = await _bookService.GetFavoriteBooks(userId);
            return Ok(favoriteBooks);
        }
    }

}