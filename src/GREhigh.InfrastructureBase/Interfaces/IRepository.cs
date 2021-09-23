using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GREhigh.Utility.Interfaces;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IRepository<out TEntity> : IEnumerable<TEntity>
        where TEntity : class, IHaveId<long> {
        public IEnumerable<T> Get<T>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        public TEntity GetByID(long id);
        public void Insert<T>(T entity)
        where T : class, IHaveId<long>;
        public void Insert<T>(IEnumerable<T> entityList)
        where T : class, IHaveId<long>;
        public void Delete(object id);
        public void Delete<T>(T entityToDelete)
        where T : class, IHaveId<long>;
        public void Update<T>(T entityToUpdate)
        where T : class, IHaveId<long>;
    }
}
