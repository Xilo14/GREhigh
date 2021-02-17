using System.Collections.Generic;
using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IPartyQueue {
        public bool Enqueue<TEntity>(Party<TEntity> party) where TEntity : Room;
        public Party<Room> Dequeue();

    }
}
