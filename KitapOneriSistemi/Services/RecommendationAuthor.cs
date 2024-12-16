using System;
using System.Collections.Generic;
using System.Linq;

public class RecommendationAuthor
{
    public static List<Book> SearchByAuthor(string authorName, List<Book> books)
    {
        if (string.IsNullOrWhiteSpace(authorName))
        {
            return new List<Book>();
        }

        return books
            .Where(book => !string.IsNullOrWhiteSpace(book.Author) && book.Author.Equals(authorName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(book => book.PublicationYear) // Sort by publication year in descending order
            .Take(30) // Take top 30 books
            .ToList();
    }
}