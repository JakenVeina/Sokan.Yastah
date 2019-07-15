namespace Microsoft.EntityFrameworkCore
{
    public interface IModelCreatingHandler<TContext>
    {
        void OnModelCreating(ModelBuilder modelBuilder);
    }
}
