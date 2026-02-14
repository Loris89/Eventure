namespace Eventure.Order.API.Utils.Pagination;

public record PaginationMetadata(int TotalPages, bool HasNextPage, bool HasPreviousPage);