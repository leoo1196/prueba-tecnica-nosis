using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

	public MovieController(IMovieService movieService)
	{
		_movieService = movieService;
	}

	[HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<Movie>))]
    public async Task<IActionResult> GetMovie([FromQuery] string title, [FromQuery] int? pageNumber)
	{
		var result = await _movieService.GetMoviesByTitleAsync(title, pageNumber);
		return result.ToStatusCodeActionResult();
	}

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] Movie movie)
    {
        var result = await _movieService.AddMovieAsync(movie);
        return result.ToStatusCodeActionResult();
    }

    [HttpDelete("{movieId}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] string movieId)
    {
        var result = await _movieService.DeleteMovieAsync(movieId);
        return result.ToStatusCodeActionResult();
    }

    [HttpPut("{movieId}")]
    public async Task<IActionResult> UpdateMovie([FromRoute] string movieId, [FromBody] Movie movie)
    {
        var result = await _movieService.UpdateMovieAsync(movieId, movie);
        return result.ToStatusCodeActionResult();
    }
}
