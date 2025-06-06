namespace Eventure.Order.API.Domain.Common;

public abstract class Entity<T> : IEntity<T>
{
    public T Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastModified { get; set; }
}
