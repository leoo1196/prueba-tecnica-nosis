using Core.Entities;
using Core.Results;

namespace Core.Services;
public interface IMovieService
{
    Task<Result> AddMovieAsync(Movie movie);

    Task<Result> DeleteMovieAsync(string movieId);

    Task<Result<PaginatedResult<Movie>>> GetMoviesByTitleAsync(string title, int? pageNumber);

    Task<Result> UpdateMovieAsync(string movieId, Movie movie);
}
