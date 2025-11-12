namespace Eventure.Order.API.Exceptions;

public class DomainRuleViolationException(string message) : Exception(message)
{
}
