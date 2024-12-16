using KitapOneriSistemi.Services;
using KitapOneriSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        // WebApplication oluşturuyoruz
        var builder = WebApplication.CreateBuilder(args);

        // PostgreSQL bağlantı dizesini appsettings.json'dan alıyoruz
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // DbContext'i Npgsql ile yapılandırıyoruz
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Servisleri DI container'a ekliyoruz
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<BookService>();
        builder.Services.AddScoped<MenuService>();
        builder.Services.AddScoped<DatabaseService>();

        // MVC controller'ları ekliyoruz
        builder.Services.AddControllers();

        // Uygulama oluşturuluyor
        var app = builder.Build();

        // Uygulama çalıştırılıyor
        var menuService = app.Services.GetRequiredService<MenuService>();
        await menuService.HandleMenu();

        app.Run();
    }
}