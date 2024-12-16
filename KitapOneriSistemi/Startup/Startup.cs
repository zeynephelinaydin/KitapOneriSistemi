using KitapOneriSistemi.Services;
using KitapOneriSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KitapOneriSistemi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // PostgreSQL bağlantı dizesini appsettings.json'dan alıyoruz
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // DbContext'i Npgsql ile yapılandırıyoruz
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Servisleri DI container'a ekliyoruz
            services.AddScoped<UserService>();
            services.AddScoped<BookService>();
            services.AddScoped<MenuService>();
            services.AddScoped<DatabaseService>();

            // MVC controller'ları ekliyoruz
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Routing kullanımı aktif ediliyor
            app.UseRouting();

            // Controller'ları map'liyoruz
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}