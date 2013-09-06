using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CmsLite.Interfaces.Services
{
    public interface IServiceBase<T> where T : class, new()
    {
        T Find(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> expression);
        IQueryable<T> FindIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetAll();
    }
}
