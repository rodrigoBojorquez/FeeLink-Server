namespace FeeLink.Application.Common.Results;

public record ListResult<T>(
    IEnumerable<T> Items,
    int TotalItems,
    int? Page = null,
    int? PageSize = null,
    int? TotalPages = null) where T : class
{
    public static ListResult<T> From<U>(ListResult<U> source, IEnumerable<T> items) where U : class
    {
        return new ListResult<T>(items, source.TotalItems, source.Page, source.PageSize, source.TotalPages);
    }
}