using System.Data.Entity;

namespace CmsLite.Interfaces.Data
{
    public interface IDbContext
    {
        IDbSet<T> GetDbSet<T>() where T : class;
        int SaveChanges();
    }
}
