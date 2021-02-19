using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IRoomRepository<TEntity> : IRepository<TEntity> where TEntity : Room {
        public IEnumerable<TEntity> GetForParty(Party<TEntity> party);
    }
}
