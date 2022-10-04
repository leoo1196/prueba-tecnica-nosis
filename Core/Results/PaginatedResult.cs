namespace Core.Results;
public class PaginatedResult<TResult>
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public IList<TResult> Results { get; set; } = null!;
}
