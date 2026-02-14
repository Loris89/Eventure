using Eventure.Order.API.Features.CreateOrder;
using Eventure.Order.API.Features.CreateOrder.Models;
using FluentValidation.TestHelper;

namespace Eventure.Order.API.UnitTests.Validators;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    #region UserId Validation

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.Empty,
            Items: [CreateValidItem()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithValidUserId_ShouldNotHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [CreateValidItem()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    #endregion

    #region Items Validation

    [Fact]
    public void Validate_WithEmptyItems_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: []
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_WithNullItems_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: null!
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    #endregion

    #region Item Validation (nested)

    [Fact]
    public void Validate_WithEmptyEventId_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [new CreateOrderItemDto(Guid.Empty, "Event", 10m, 1)]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].EventId");
    }

    [Fact]
    public void Validate_WithEmptyEventName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [new CreateOrderItemDto(Guid.NewGuid(), "", 10m, 1)]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].EventName");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidQuantity_ShouldHaveError(int quantity)
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [new CreateOrderItemDto(Guid.NewGuid(), "Event", 10m, quantity)]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void Validate_WithNegativeUnitPrice_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [new CreateOrderItemDto(Guid.NewGuid(), "Event", -10m, 1)]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");
    }

    [Fact]
    public void Validate_WithZeroUnitPrice_ShouldPass()
    {
        // Free events should be allowed!
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [new CreateOrderItemDto(Guid.NewGuid(), "Free Event", 0m, 1)]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Items[0].UnitPrice");
    }

    #endregion

    #region Full Valid Command

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateOrderCommand(
            UserId: Guid.NewGuid(),
            Items: [
                new CreateOrderItemDto(Guid.NewGuid(), "Event A", 25.00m, 2),
                new CreateOrderItemDto(Guid.NewGuid(), "Event B", 15.50m, 1)
            ]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    // Helper
    private static CreateOrderItemDto CreateValidItem() =>
        new(Guid.NewGuid(), "Test Event", 10.00m, 1);
}
