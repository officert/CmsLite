using System;
using CmsLite.Interfaces.Data;

namespace CmsLite.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IDbContext Context { get { return _context; } }
    }
}
