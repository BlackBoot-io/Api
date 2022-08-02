using Avn.Data.Extensions;
using Avn.Shared.Extentions;

namespace Avn.Data.Context;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext() { }
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().AreUnicode(false);
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterAllEntities<IEntity>(typeof(object).Assembly);
    }
}

