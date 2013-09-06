using System.Data.Entity;

namespace CmsLite.Interfaces.Data
{
    public interface IUnitOfWork
    {
        IDbContext Context { get; }
        void Commit();
    }
}
