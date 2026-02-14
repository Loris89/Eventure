using Eventure.Order.API.Features.GetAllOrdersForUser;
using Eventure.Order.API.Features.GetAllOrdersForUser.Models;
using FluentValidation.TestHelper;

namespace Eventure.Order.API.UnitTests.Validators;

public class GetAllOrdersForUserRequestValidatorTests
{
    private readonly GetAllOrdersForUserRequestValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithPageLessThanOrEqualTo0_ShouldHaveError(int page)
    {
        // Arrange
        var query = new GetAllOrdersForUserRequest(
            Page: page,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeNotInRange_ShouldHaveError(int pageSize)
    {
        // Arrange
        var query = new GetAllOrdersForUserRequest(
            Page: 1,
            PageSize: pageSize
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithValidPagingInfo_ShouldNotHaveError()
    {
        // Arrange
        var query = new GetAllOrdersForUserRequest(
            Page: 1,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
