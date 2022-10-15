namespace Avn.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().AreUnicode(false);
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(X => X.Email).IsUnique();

        modelBuilder.RegisterAllEntities<IEntity>(typeof(User).Assembly);
        modelBuilder.AddPluralizingTableNameConvention();
        
    }
}