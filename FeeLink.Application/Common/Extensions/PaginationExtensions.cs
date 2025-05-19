namespace FeeLink.Application.Common.Extensions;

public static class PaginationExtensions
{
    public static int GetTotalPages(this int totalItems, int pageSize)
    {
        return (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}