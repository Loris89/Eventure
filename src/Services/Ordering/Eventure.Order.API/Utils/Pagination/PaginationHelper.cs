namespace Eventure.Order.API.Utils.Pagination;

public static class PaginationHelper
{
    public static PaginationMetadata Calculate(int totalCount, int page, int pageSize)
    {
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        bool hasNextPage = page < totalPages;
        bool hasPreviousPage = page > 1;

        return new PaginationMetadata(totalPages, hasNextPage, hasPreviousPage);
    }
}
