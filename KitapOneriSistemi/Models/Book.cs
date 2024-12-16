using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;  // ICollection için gerekli
using KitapOneriSistemi.Models;  // Eğer UserBook burada tanımlandıysa


public class Book
{
    [Key] // ISBN'yi birincil anahtar olarak belirliyoruz.
    public string ISBN { get; set; }

    [Required] // Kitap adı zorunlu
    [StringLength(255)] // Kitap adı uzunluğunu sınırlıyoruz
    public string Name { get; set; }

    public double AverageRating { get; set; }

    [StringLength(1000)] // Açıklama uzunluğunu sınırlıyoruz
    public string? Explanation { get; set; }

    [Required] // Yazar adı zorunlu
    [StringLength(255)] // Yazar adı uzunluğunu sınırlıyoruz
    public string Author { get; set; }

    [Range(1000, 9999)] // Yayın yılı 4 haneli olmalı
    public int PublicationYear { get; set; } // Yayın yılı

    [Range(1, int.MaxValue)] // Sayfa sayısı 1'den büyük olmalı
    public int PagesCount { get; set; }

    [Required] // Kitap türü zorunlu
    [StringLength(50)] // Kitap türü uzunluğunu sınırlıyoruz
    public string BookType { get; set; }

    // UserBook ile ilişkili koleksiyon
    public ICollection<UserBook> UserBook { get; set; }

}