namespace Eventure.Order.API.Domain.Common;

// Eredita da IAggregate quindi disporrà dell'elenco
// degli eventi di dominio e del metodo per toglierli.
// Inoltre eredita da IEntity<T> quindi sarà un'entità
// il cui identificativo è di tipo T.
public interface IAggregate<T> : IAggregate, IEntity<T>
{

}

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    IDomainEvent[] ClearDomainEvents();
}