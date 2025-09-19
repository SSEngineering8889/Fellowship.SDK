using Fellowship.SDK.Extensions;
using Fellowship.SDK.Filters;
using Fellowship.SDK.Interfaces;
using Fellowship.SDK.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

// Setup DI container with logging
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddFellowshipSdk(options => options.ApiKey = "YOUR_API_KEY");

var serviceProvider = services.BuildServiceProvider();

// Get client from DI (using interface for better DI practices)
var client = serviceProvider.GetRequiredService<IFellowshipClient>();

Console.WriteLine("=== Fellowship SDK Demo with Logging ===\n");

Console.WriteLine("1. Getting movies with runtime >= 160 minutes...");
var longMovies = await client.Movies.GetAllAsync(
    filters: new[] {
        new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThanOrEqual, "160")
    }
);
Console.WriteLine($"Found {longMovies.Count()} long movies\n");

Console.WriteLine("2. Getting all movies...");
var getAllMovies = await client.Movies.GetAllAsync();
Console.WriteLine($"Found {getAllMovies.Count()} total movies\n");

Console.WriteLine("3. Getting quotes mentioning 'ring'...");
var ringQuotes = await client.Quotes.GetAllAsync(limit:2,page:2,
    filters: new[] {
        new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "/ring/i")
    }
);
Console.WriteLine($"Found {ringQuotes.Count()} quotes about rings\n");

Console.WriteLine("4. Sample quotes:");
Console.WriteLine(JsonSerializer.Serialize(ringQuotes.Take(3), new JsonSerializerOptions { WriteIndented = true }));

Console.ReadLine();
