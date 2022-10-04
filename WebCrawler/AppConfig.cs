namespace WebCrawler;
internal class AppConfig
{
    public const string Section = nameof(AppConfig);

    public string TsvFileUrl { get; set; } = null!;
    public int TsvIdColumn { get; set; }
    public int TsvTypeColumn { get; set; }
    public string OmdbApi { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public int ProcessTimer { get; set; }
}
