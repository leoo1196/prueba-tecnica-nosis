using Core.Entities;
using Core.Errors;
using Core.Results;
using Core.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;
public class MovieService : IMovieService
{
    private readonly ApplicationContext _context;

    private const string notFound = "Pelicula no encontrada";
    private const string noMatch = "No se encontraron coincidencias";
    private const string invalidId = "Los id de la pelicula no coinciden";
    private const string invalidPageNumber = "El número de pagina no puede ser menor a 1";

    public MovieService(ApplicationContext context) => _context = context;

    public async Task<Result> AddMovieAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return Result.Empty();
    }

    public async Task<Result> DeleteMovieAsync(string movieId)
    {
        var movie = await _context.Movies.FindAsync(movieId);

        if (movie is null)
            return new NotFoundError(notFound);

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return Result.Empty();
    }

    public async Task<Result<PaginatedResult<Movie>>> GetMoviesByTitleAsync(string title, int? pageNumber)
    {
        if (pageNumber.HasValue && pageNumber.Value < 1)
            return new BadRequestError(invalidPageNumber);

        pageNumber ??= 1;

        var query = _context.Movies.Where(movie => movie.Title.Contains(title));
        var totalRecords = await query.CountAsync();

        if (totalRecords == 0)
            return new NotFoundError(noMatch);

        var movies = await query.Skip((pageNumber.Value - 1) * 5).Take(5).ToListAsync();

        return new PaginatedResult<Movie>
        {
            Results = movies,
            PageNumber = pageNumber.Value,
            TotalPages = totalRecords % 5 == 0 ? totalRecords / 5 : (totalRecords / 5) + 1,
            TotalRecords = totalRecords
        };
    }

    public async Task<Result> UpdateMovieAsync(string movieId, Movie movie)
    {
        if (await _context.Movies.AsNoTracking().FirstOrDefaultAsync(e => e.Id == movieId) is null)
            return new NotFoundError(notFound);

        if (movieId != movie.Id)
            return new BadRequestError(invalidId);

        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();

        return Result.Empty();
    }
}
