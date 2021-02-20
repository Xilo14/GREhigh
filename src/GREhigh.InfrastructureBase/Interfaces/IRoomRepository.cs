using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IRoomRepository<TEntity> : IRoomRepository where TEntity : Room {
        public new IEnumerable<TEntity> GetForParty(Party party);
    }
    public interface IRoomRepository : IRepository<Room> {
        public IEnumerable<Room> GetForParty(Party party);
    }
}
