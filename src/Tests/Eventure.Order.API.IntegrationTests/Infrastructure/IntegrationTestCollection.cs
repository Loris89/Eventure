namespace Eventure.Order.API.IntegrationTests.Infrastructure;

// Le classi che usano questa collection condivideranno lo stesso container postgres
// per evitare di avviarne più di uno.
[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<PostgresContainerFixture>
{
    public const string Name = "integration-tests";
}
