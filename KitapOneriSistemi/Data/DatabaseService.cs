using System;
using System.Collections.Generic;
using Npgsql;

public class DatabaseService
{
    private string _connectionString = "Host=pg-bc53324-projekitap.j.aivencloud.com;Port=27533;Username=avnadmin;Password=AVNS_VEAeDT4vEQFLvwFwTxa;Database=defaultdb;SSL Mode=Require;";

    // Türlere göre kitapları getirir
    public List<Book> GetBooksByGenres(List<string> genres)
    {
        List<Book> books = new List<Book>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Türleri dinamik olarak sorguya ekliyoruz
            var genrePlaceholders = string.Join(", ", genres.ConvertAll(g => $"@genre{genres.IndexOf(g)}"));
            var query = $"SELECT * FROM books WHERE book_type IN ({genrePlaceholders})";

            using (var command = new NpgsqlCommand(query, connection))
            {
                // Türleri parametre olarak ekliyoruz
                for (int i = 0; i < genres.Count; i++)
                {
                    command.Parameters.AddWithValue($"@genre{i}", genres[i]);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Null kontrolü ve kitap bilgilerini alıyoruz
                        string author = reader.IsDBNull(reader.GetOrdinal("author")) ? "Yazar Bilgisi Yok" : reader.GetString(reader.GetOrdinal("author"));
                        string name = reader.GetString(reader.GetOrdinal("name"));

                        // 'Explanation' sütununu kontrol et ve null ise boş bir değer ata
                        string explanation = reader.IsDBNull(reader.GetOrdinal("explanation")) ? null : reader.GetString(reader.GetOrdinal("explanation"));

                        string bookType = reader.GetString(reader.GetOrdinal("book_type"));

                        books.Add(new Book
                        {
                            Name = name,
                            Author = author,
                            Explanation = explanation,
                            BookType = bookType
                        });
                    }
                }
            }
        }
        return books;
    }

    // Kullanıcıya arama yapma imkanı sağlayan eski fonksiyon
    public List<Book> GetBooksFromDatabase(string searchTerm)
    {
        List<Book> books = new List<Book>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM books WHERE name ILIKE @searchTerm OR author ILIKE @searchTerm OR explanation ILIKE @searchTerm", connection))
            {
                command.Parameters.AddWithValue("searchTerm", "%" + searchTerm + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string author = reader.IsDBNull(reader.GetOrdinal("author")) ? "Yazar Bilgisi Yok" : reader.GetString(reader.GetOrdinal("author"));
                        string bookName = reader.GetString(reader.GetOrdinal("name"));

                        // 'Explanation' sütununu kontrol et ve null ise boş bir değer ata
                        string explanation = reader.IsDBNull(reader.GetOrdinal("explanation")) ? null : reader.GetString(reader.GetOrdinal("explanation"));

                        books.Add(new Book
                        {
                            Name = bookName,
                            Author = author,
                            Explanation = explanation
                        });
                    }
                }
            }
        }
        return books;
    }

    // Rating'e göre en yüksek 30 kitabı getirir
    public List<Book> GetTopRatedBooks(int i)
    {
        List<Book> books = new List<Book>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // books ve ratings tablosunu birleştiriyoruz
            var query = @"
                SELECT b.ISBN, b.name, b.author, b.explanation, b.book_type, b.publication_year, 
                       b.pages_count, r.average_rating
                FROM books b
                JOIN ratings r ON b.ISBN = r.ISBN
                ORDER BY r.average_rating DESC
                LIMIT 30"; // En yüksek rating'e göre sırala ve ilk 30 kitabı al

            using (var command = new NpgsqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string author = reader.IsDBNull(reader.GetOrdinal("author")) ? "Yazar Bilgisi Yok" : reader.GetString(reader.GetOrdinal("author"));
                        string name = reader.GetString(reader.GetOrdinal("name"));

                        // 'Explanation' sütununu kontrol et ve null ise boş bir değer ata
                        string explanation = reader.IsDBNull(reader.GetOrdinal("explanation")) ? null : reader.GetString(reader.GetOrdinal("explanation"));
                        string bookType = reader.GetString(reader.GetOrdinal("book_type"));
                        double averageRating = reader.GetDouble(reader.GetOrdinal("average_rating"));

                        books.Add(new Book
                        {
                            Name = name,
                            Author = author,
                            Explanation = explanation,
                            BookType = bookType,
                            AverageRating = averageRating
                        });
                    }
                }
            }
        }
        return books;
    }
}