using System.Text.Json.Serialization;

namespace Core.Entities;
public class Movie
{
    [JsonPropertyName("imdbID")]
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    [JsonPropertyName("imdbRating")]
    public string ImdbRating { get; set; } = null!;
    public string Plot { get; set; } = null!;
    public string Poster { get; set; } = null!;
}
