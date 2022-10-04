using Core.Entities;
using Infrastructure;
using IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Text.Json;
using WebCrawler;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json"))
    .ConfigureServices((ctx, services) =>
    {
        var appConfig = ctx.Configuration.GetSection(AppConfig.Section).Get<AppConfig>();

        services.AddSingleton(appConfig);
        services.AddDbContexts(ctx.Configuration);
    })
    .Build();

await MainMethod(host.Services);

await host.RunAsync();

static async Task MainMethod(IServiceProvider serviceProvider)
{
    Console.WriteLine("Running WebCrawler");
    using var scope = serviceProvider.CreateScope();

    var appConfig = scope.ServiceProvider.GetRequiredService<AppConfig>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

    await db.Database.EnsureCreatedAsync();

    // el proceso se ejecuta cada 30 minutos
    var timer = new PeriodicTimer(TimeSpan.FromMinutes(appConfig.ProcessTimer));

    do
    {
        await RunProcessAsync(db, appConfig);
    } while (await timer.WaitForNextTickAsync());
}

static async Task RunProcessAsync(ApplicationContext context, AppConfig appConfig)
{
    using var tsvFile = await GetTsvFile(appConfig.TsvFileUrl);
    using var streamReader = new StreamReader(tsvFile);
    using var client = new HttpClient();

    var counter = 0;

    Console.WriteLine("Comienza lectura del archivo .tsv");
    while (true)
    {
        var line = await streamReader.ReadLineAsync();

        if (line is null)
            break;

        if (counter++ == 0)
            continue;

        var result = await ProcessLineAsync(line, client, context, appConfig);

        if (result == ProcessResult.LimitReached)
            break;
    }

    tsvFile.Close();
    streamReader.Close();
    File.Delete("temp.tsv");
}

static async Task<FileStream> GetTsvFile(string urlFile)
{
    using var client = new HttpClient();
    Console.WriteLine("Descargando archivo .tsv");
    using var fileStream = await client.GetStreamAsync(urlFile);
    using var gzStream = new GZipStream(fileStream, CompressionMode.Decompress);
    var tempFile = File.Create("temp.tsv");

    Console.WriteLine("Descomprimiendo archivo .tsv");
    await gzStream.CopyToAsync(tempFile);

    tempFile.Position = 0;
    return tempFile;
}

static async Task<ProcessResult> ProcessLineAsync(string line, HttpClient httpClient, ApplicationContext context, AppConfig appConfig)
{
    var values = line.Split("\t");

    if (values[appConfig.TsvTypeColumn] != "movie")
        return ProcessResult.NotProcessed;

    var movieId = values[appConfig.TsvIdColumn];

    if (await context.Movies.FindAsync(movieId) is not null)
        return ProcessResult.NotProcessed;

    return await DownloadAndSaveMovieAsync(httpClient, context, movieId, appConfig);
}

static async Task<ProcessResult> DownloadAndSaveMovieAsync(HttpClient httpClient, ApplicationContext context, string movieId, AppConfig appConfig)
{
    try
    {
        Console.WriteLine($"Descargando pelicula con Id: {movieId}");
        var response = await httpClient.GetAsync($"{appConfig.OmdbApi}?i={movieId}&apikey={appConfig.ApiKey}");

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(content);

                context.Movies.Add(movie!);
                await context.SaveChangesAsync();

                return ProcessResult.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando la pelicula con Id: {movieId}. ErrorMessage: {ex.Message}");
                return ProcessResult.Error;
            }
        }
        else
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Limite diario de requests alcanzado");
                return ProcessResult.LimitReached;
            }

            if (response.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                Console.WriteLine("Error inesperado del servidor");

            return ProcessResult.Error;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en request a la api. ErrorMessage: {ex.Message}");
        return ProcessResult.Error;
    }
}