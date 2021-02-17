using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;
using GREhigh.Infrastructure.Interfaces;

namespace GREhigh.RoomStaffBase.Interfaces {
    public interface IRoomRepository<TEntity> : IRepository<TEntity> where TEntity : Room {
        public IEnumerable<TEntity> GetForParty(Party<TEntity> party);
    }
}
