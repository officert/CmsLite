using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace CmsLite.Utilities.UnitTesting
{
    public class InMemoryDbSet<T> : IDbSet<T> where T : class, new()
    {
        private readonly ICollection<T> _collection = new Collection<T>();

        public T Add(T entity)
        {
            _collection.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            throw new NotImplementedException();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            return new T();
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<T> Local
        {
            get { throw new NotImplementedException(); }
        }

        public T Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return this._collection.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _collection.AsQueryable().Provider; }
        }
    }
}
