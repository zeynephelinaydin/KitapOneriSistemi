using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

public class RecommendationExplanation
{
    private MLContext _mlContext;

    public RecommendationExplanation()
    {
        _mlContext = new MLContext();
    }

    public static List<BookRecommendationResult> GetBookRecommendations(string userInput, List<Book> books)
    {
        var mlContext = new MLContext();
        var bookData = books.Select(book => new BookInput { BookName = book.Name, Explanation = book.Explanation }).ToList();
        var bookDataView = mlContext.Data.LoadFromEnumerable(bookData);
        var textPipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(BookInput.Explanation));
        var transformer = textPipeline.Fit(bookDataView);
        var transformedData = transformer.Transform(bookDataView);
        var transformedBooks = mlContext.Data.CreateEnumerable<BookOutput>(transformedData, reuseRowObject: false).ToList();

        var recommendations = new List<BookRecommendationResult>();
        var userInputVector = FeaturizeUserInput(userInput, transformer);

        foreach (var book in transformedBooks)
        {
            var similarityScore = CosineSimilarity(userInputVector, book.Features);
            recommendations.Add(new BookRecommendationResult { BookName = book.BookName, SimilarityScore = similarityScore });
        }

        return recommendations.OrderByDescending(r => r.SimilarityScore).Take(10).ToList();
    }

    private static float[] FeaturizeUserInput(string userInput, ITransformer transformer)
    {
        var mlContext = new MLContext();
        var userInputData = new List<BookInput> { new BookInput { Explanation = userInput } };
        var userInputDataView = mlContext.Data.LoadFromEnumerable(userInputData);
        var transformedUserInput = transformer.Transform(userInputDataView);
        var userInputFeatures = mlContext.Data.CreateEnumerable<BookOutput>(transformedUserInput, reuseRowObject: false).FirstOrDefault();
        return userInputFeatures?.Features;
    }

    private static double CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        var dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
        var magnitudeA = Math.Sqrt(vectorA.Sum(x => x * x));
        var magnitudeB = Math.Sqrt(vectorB.Sum(x => x * x));
        if (magnitudeA == 0 || magnitudeB == 0) return 0;
        return dotProduct / (magnitudeA * magnitudeB);
    }
}

public class BookInput
{
    public string BookName { get; set; }
    public string Explanation { get; set; }
}

public class BookOutput : BookInput
{
    public float[] Features { get; set; }
}

public class BookRecommendationResult
{
    public string BookName { get; set; }
    public double SimilarityScore { get; set; }
}
