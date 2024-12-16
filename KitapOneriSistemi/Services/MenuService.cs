using KitapOneriSistemi.Services;
using KitapOneriSistemi.Models;
using KitapOneriSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using KitapOneriSistemi.Enums;
using KitapOneriSistemi.Data; 
using Microsoft.EntityFrameworkCore;

namespace KitapOneriSistemi.Services
{
    public class MenuService
    {
        private readonly UserService _userService;
        private readonly DatabaseService _databaseService;
        private readonly BookService _bookService;
        private readonly ApplicationDbContext _context;  // YourDbContext'i burada ekliyoruz

        public MenuService(UserService userService, DatabaseService databaseService, BookService bookService, ApplicationDbContext context)
        {
            _userService = userService;
            _databaseService = databaseService;
            _bookService = bookService;
            _context = context;  // _context'i constructor'a ekliyoruz
        }


        public async Task HandleMenu()
        {
            // Ana Menü
            Console.WriteLine("Hoş geldiniz! Yapmak istediğiniz işlemi seçin:");
            Console.WriteLine("1 - Kullanıcı Kaydı");
            Console.WriteLine("2 - Giriş Yap");
            Console.WriteLine("3 - Kitap Önerisi");
            Console.WriteLine("4 - İstek Listesi");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                await RegisterUser();
            }
            else if (choice == "2")
            {
                await LoginUser();
            }
            else if (choice == "3")
            {
                await SearchBooksMenu();
            }
            else if (choice == "4")
            {
                await ViewWishlist();
            }
            else
            {
                // Seçim geçersiz olduğunda mesaj
                Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
            }
        }

        private async Task RegisterUser()
        {
            // Kullanıcı kaydı
            Console.WriteLine("Kullanıcı adı girin:");
            var username = Console.ReadLine();
            Console.WriteLine("Email girin:");
            var email = Console.ReadLine();
            Console.WriteLine("Şifre girin:");
            var password = Console.ReadLine();

            var isRegistered = await _userService.RegisterUser(username, email, password);
            if (isRegistered)
            {
                Console.WriteLine("Kullanıcı kaydedildi!");
            }
            else
            {
                Console.WriteLine("Kullanıcı kaydedilemedi. Kullanıcı adı ya da e-posta zaten mevcut.");
            }
        }

        private async Task LoginUser()
        {
            // Giriş işlemi
            Console.WriteLine("Giriş yapmak için email ve şifreyi girin:");

            Console.WriteLine("Email:");
            var loginEmail = Console.ReadLine();
            Console.WriteLine("Şifre:");
            var loginPassword = Console.ReadLine();

            var isLoggedIn = await _userService.LoginUser(loginEmail, loginPassword);
            if (isLoggedIn)
            {
                Console.WriteLine("Başarıyla giriş yapıldı.");

                // Türleri gösterelim
                Console.WriteLine("Lütfen ilgilendiğiniz kitap türlerini seçin:");
                Console.WriteLine("1 - Türk Klasikleri");
                Console.WriteLine("2 - Romantik");
                Console.WriteLine("3 - Korku Gerilim");
                Console.WriteLine("4 - Macera");
                Console.WriteLine("5 - Polisiye");
                Console.WriteLine("6 - Fantastik");
                Console.WriteLine("7 - Bilim Kurgu");
                Console.WriteLine("8 - Türk Romanı");
                Console.WriteLine("9 - Dünya Roman");
                Console.WriteLine("0 - Seçim yapmadan geç");

                var genreChoice = Console.ReadLine();
                await HandleGenres(genreChoice);
            }
            else
            {
                Console.WriteLine("Giriş yapılamadı. Lütfen email ve şifrenizi kontrol edin.");
            }
        }

        private async Task HandleGenres(string genreChoice)
        {
            if (genreChoice != "0")
            {
                // Kullanıcı türleri seçmişse, türleri listele
                List<string> selectedGenres = new List<string>();

                if (genreChoice.Contains("1")) selectedGenres.Add("Türk Klasikleri");
                if (genreChoice.Contains("2")) selectedGenres.Add("Romantik");
                if (genreChoice.Contains("3")) selectedGenres.Add("Korku Gerilim");
                if (genreChoice.Contains("4")) selectedGenres.Add("Macera");
                if (genreChoice.Contains("5")) selectedGenres.Add("Polisiye");
                if (genreChoice.Contains("6")) selectedGenres.Add("Fantastik");
                if (genreChoice.Contains("7")) selectedGenres.Add("Bilim Kurgu");
                if (genreChoice.Contains("8")) selectedGenres.Add("Türk Romanı");
                if (genreChoice.Contains("9")) selectedGenres.Add("Dünya Roman");

                // Seçilen türlerde kitapları alalım
                var booksByGenre = _databaseService.GetBooksByGenres(selectedGenres);

                // Kitapları gösterelim
                if (booksByGenre.Any())
                {
                    Console.WriteLine($"Seçtiğiniz türlerdeki kitaplar:");
                    foreach (var book in booksByGenre)
                    {
                        Console.WriteLine($"Kitap Adı: {book.Name}, Yazar: {book.Author}, Tür: {book.BookType}");
                    }

                    // Kullanıcıya kitap ismi girmesi için soralım
                    Console.WriteLine("İstek listesine eklemek istediğiniz kitabın adını girin:");
                    string bookName = Console.ReadLine();

                    // Kitap adı geçerli mi kontrol edelim
                    var bookToAdd =
                        booksByGenre.FirstOrDefault(b => b.Name.Equals(bookName, StringComparison.OrdinalIgnoreCase));
                    if (bookToAdd != null)
                    {
                        // Kitap geçerli, istek listesine ekleyelim
                        var userId = 1; // Bu kısmı giriş yapan kullanıcı id'siyle değiştirebilirsiniz
                        var result = await _bookService.AddToFavorites(userId, bookToAdd.ISBN);
                        if (result)
                        {
                            Console.WriteLine("Kitap istek listesine başarıyla eklendi.");
                        }
                        else
                        {
                            Console.WriteLine("Kitap zaten istek listesinde mevcut.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Kitap bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine("Seçtiğiniz türlerde kitap bulunamadı.");
                }
            }
            else
            {
                // Kullanıcı "0" seçerse tüm türlerden en yüksek 30 rating'e sahip kitapları getir
                Console.WriteLine("En yüksek 30 rating'e sahip kitaplar:");
                ShowTopRatedBooks();
            }
        }

        private async Task ViewWishlist()
        {
            // Kullanıcıdan ID alarak, istek listesini gösterelim
            Console.WriteLine("İstek listenizi görmek için kullanıcı ID'si girin:");
            var userIdInput = Console.ReadLine();
            if (int.TryParse(userIdInput, out int userId))
            {
                await ShowWishlist(userId);
            }
            else
            {
                Console.WriteLine("Geçersiz kullanıcı ID'si.");
            }
        }

        private async Task ShowWishlist(int userId)
        {
            var favoriteBooks = await _bookService.GetFavoriteBooks(userId);

            if (favoriteBooks.Any())
            {
                Console.WriteLine("İstek listenizdeki kitaplar:");
                foreach (var book in favoriteBooks)
                {
                    Console.WriteLine($"Kitap Adı: {book.Name}, Yazar: {book.Author}");
                }
            }
            else
            {
                Console.WriteLine("İstek listenizde hiç kitap bulunmamaktadır.");
            }
        }

        private async Task SearchBooksMenu()
        {
            // Kitap arama yapmak için kullanıcıdan terim alın
            Console.WriteLine("Kitap arama yapmak için kitap adı, yazar adı veya açıklama yazabilirsiniz.");
            Console.WriteLine("Arama yapmak istediğiniz terimi girin:");

            string searchInput = Console.ReadLine(); // Kullanıcıdan arama terimi alın

            var books = _databaseService.GetBooksFromDatabase(searchInput); // Arama terimiyle kitapları al

            // Kitap adı, yazar adı ve açıklamaya göre arama yapacak metod
            var searchResults = SearchBooks(searchInput, books);

            if (searchResults.Any())
            {
                Console.WriteLine($"Toplamda {searchResults.Count} kitap bulundu:");
                foreach (var book in searchResults)
                {
                    Console.WriteLine(
                        $"Kitap Adı: {book.Name}, Yazar: {book.Author}, Yayın Yılı: {book.PublicationYear}");
                }
            }
            else
            {
                Console.WriteLine("Aradığınız kritere uyan kitap bulunamadı.");
            }
        }

        // Kitapları aramak için kullanılan metod
        private List<Book> SearchBooks(string searchTerm, List<Book> books)
        {
            // Boş arama yapılmışsa, tüm kitapları gösterelim
            if (string.IsNullOrEmpty(searchTerm))
                return books;

            return books.Where(b => b.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    b.Explanation.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void ShowTopRatedBooks()
        {
            // PostgreSQL bağlantı bilgilerini ayarla
            string connectionString =
                "Host=pg-bc53324-projekitap.j.aivencloud.com;Port=27533;Username=avnadmin;Password=AVNS_VEAeDT4vEQFLvwFwTxa;Database=defaultdb;SSL Mode=Require;";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Rating'e göre sıralanmış ilk 30 kitabı sorgula
                string query = "SELECT b.name, b.book_type, b.author, b.isbn, r.average_rating " +
                               "FROM books b JOIN ratings r ON b.isbn = r.isbn " +
                               "ORDER BY r.average_rating DESC LIMIT 30;";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    // Kitapları listeye al
                    var books = new (string Name, string BookType, string Author, string ISBN, double Rating)[30];
                    int counter = 0;

                    while (reader.Read())
                    {
                        books[counter] = (
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                            reader.GetString(3),
                            reader.GetDouble(4)
                        );
                        counter++;
                    }

                    // Kitapları numaralandırarak yazdır
                    Console.WriteLine("\nEn yüksek 30 rating'e sahip kitaplar:");
                    for (int i = 0; i < books.Length; i++)
                    {
                        Console.WriteLine(
                            $"{i + 1} - {books[i].Name} - {books[i].BookType} - Yazar: {books[i].Author} - ISBN: {books[i].ISBN} - Rating: {books[i].Rating}");
                    }

                    // Kullanıcıdan bir kitap seçmesini iste
                    Console.WriteLine("\nLütfen bu listeden bir kitap seçiniz. Kitap numarasını girin:");
                    var selectedBookIndex = Console.ReadLine();

                    // Seçilen kitabın sırasına göre ISBN numarasını al
                    int bookIndex = Int32.Parse(selectedBookIndex) - 1;
                    var selectedBookInfo = books[bookIndex];

                    Console.WriteLine(
                        $"Seçilen kitap: {selectedBookInfo.Name} - {selectedBookInfo.BookType} - Yazar: {selectedBookInfo.Author} - ISBN: {selectedBookInfo.ISBN}");

                    // Kullanıcıdan istek listesine eklemek isteyip istemediğini sor
                    Console.WriteLine("\nBu kitabı istek listenize eklemek ister misiniz? (Evet/Hayır)");
                    var addToWishlist = Console.ReadLine();

                    if (addToWishlist.Equals("Evet", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Lütfen kullanıcı ID'nizi girin:");
                        int userId = Int32.Parse(Console.ReadLine());

                        // Kitabı istek listesine ekle
                        AddBookToWishlist(userId, selectedBookInfo.ISBN);
                    }

                    // Cosine Similarity hesapla ve 5 en benzer kitabı öner
                    var recommendedBooks = GetRecommendedBooksFromDatabase(selectedBookInfo);

                    Console.WriteLine("\nBu kitapları da beğenebilirsiniz:");
                    foreach (var book in recommendedBooks)
                    {
                        Console.WriteLine(
                            $"- {book.Name} - {book.BookType} - Yazar: {book.Author} - ISBN: {book.ISBN}");
                    }
                }
            }
        }

        public async Task AddBookToWishlist(int userId, string isbn)
        {
            // Kullanıcı zaten kitabı istek listesine eklemiş mi kontrol et
            var existingUserBook = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.ISBN == isbn);

            // Eğer kitap zaten varsa, istek listesine tekrar eklemeyelim
            if (existingUserBook != null)
            {
                // Kitap zaten istek listesinde olabilir. Burada isterseniz güncelleyebilirsiniz.
                existingUserBook.Status = KitapDurumu.Wishlist;
                existingUserBook.AddedDate = DateTime.UtcNow;  // Tarihi güncelleyebilirsiniz
                await _context.SaveChangesAsync();
                return;
            }

            // Yeni kitap eklemek
            var userBook = new UserBook
            {
                UserId = userId,
                ISBN = isbn,
                Status = KitapDurumu.Wishlist,  // İstek listesine eklemek için status 1 (Wishlist)
                AddedDate = DateTime.UtcNow     // Şu anki tarihi ekliyoruz
            };

            // Kullanıcı kitabı istek listesine ekliyoruz
            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();
        }


        private (string Name, string BookType, string Author, string ISBN)[] GetRecommendedBooksFromDatabase(
            (string Name, string BookType, string Author, string ISBN, double Rating) selectedBook)
        {
            string connectionString =
                "Host=pg-bc53324-projekitap.j.aivencloud.com;Port=27533;Username=avnadmin;Password=AVNS_VEAeDT4vEQFLvwFwTxa;Database=defaultdb;SSL Mode=Require;";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT b.name, b.book_type, b.author, b.isbn FROM books b;";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var allBooks = new List<(string Name, string BookType, string Author, string ISBN)>();

                    while (reader.Read())
                    {
                        allBooks.Add((
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                            reader.GetString(3)
                        ));
                    }

                    // 1. Aynı yazardan yalnızca 1 kitap al
                    var authorBooks = allBooks
                        .Where(b => b.Author.Equals(selectedBook.Author, StringComparison.OrdinalIgnoreCase) &&
                                    b.ISBN != selectedBook.ISBN)
                        .Take(1) // Yalnızca 1 kitap öner
                        .ToArray();

                    // 2. Tür benzerliğine göre sıralama ve farklı yazarlardan 4 kitap al
                    var genreBooks = allBooks
                        .Where(b => b.BookType.Equals(selectedBook.BookType, StringComparison.OrdinalIgnoreCase) &&
                                    b.ISBN != selectedBook.ISBN)
                        .Where(b => !authorBooks.Any(ab =>
                            ab.Author.Equals(b.Author, StringComparison.OrdinalIgnoreCase))) // Aynı yazarı dışla
                        .OrderByDescending(b =>
                            CosineSimilarity(selectedBook.BookType, b.BookType)) // Tür benzerliğine göre sıralama
                        .Take(4) // En benzer 4 kitap
                        .ToArray();

                    // 3. 5 kitap olacak şekilde tür benzerliğinden 4 kitap + aynı yazardan 1 kitap öner
                    var recommendedBooks = authorBooks.Concat(genreBooks).ToArray();

                    // 5 kitap olacak şekilde döndürüyoruz
                    return recommendedBooks;
                }
            }
        }

// Cosine Similarity fonksiyonu örneği
        private double CosineSimilarity(string bookType1, string bookType2)
        {
            // Bu basit örnek sadece uzunluk karşılaştırması yapar, ihtiyaca göre detaylı bir benzerlik fonksiyonu eklenebilir
            return bookType1.Equals(bookType2, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.0;
        }
    }
}