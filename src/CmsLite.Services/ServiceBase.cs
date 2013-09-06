using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CmsLite.Interfaces.Data;

namespace CmsLite.Services
{
    public abstract class ServiceBase<T> where T : class, new()
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return UnitOfWork.Context.GetDbSet<T>().FirstOrDefault(expression);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return UnitOfWork.Context.GetDbSet<T>().Where(expression);
        }

        public IQueryable<T> FindIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            var results = GetAll().AsQueryable();
            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    results = results.Include(include);
                }
            }
            return results;
        }

        public IQueryable<T> GetAll()
        {
            return UnitOfWork.Context.GetDbSet<T>();
        }
    }
}
