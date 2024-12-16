using System;
using System.Collections.Generic;
using System.Linq;

public static class RecommendationBook
{
    public static List<Book> SearchByBookName(string bookName, List<Book> books)
    {
        return books.Where(b => b.Name.Contains(bookName, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}