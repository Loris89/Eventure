using Eventure.Order.API.Utils.Pagination;
using Shouldly;

namespace Eventure.Order.API.UnitTests.Pagination;

public class PaginationHelperTests
{
    [Fact]
    public void Calculate_MiddlePage()
    {
        var result = PaginationHelper.Calculate(100, 2, 10);

        result.TotalPages.ShouldBe(10);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeTrue();
    }

    [Fact]
    public void Calculate_FirstPage()
    {
        var result = PaginationHelper.Calculate(100, 1, 10);

        result.TotalPages.ShouldBe(10);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Fact]
    public void Calculate_LastPage()
    {
        var result = PaginationHelper.Calculate(100, 10, 10);

        result.TotalPages.ShouldBe(10);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeTrue();
    }

    [Fact]
    public void Calculate_NoResults()
    {
        var result = PaginationHelper.Calculate(0, 1, 10);

        result.TotalPages.ShouldBe(0);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }
}
