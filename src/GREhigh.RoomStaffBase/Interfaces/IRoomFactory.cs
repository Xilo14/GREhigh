using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.RoomStaffBase.Interfaces {
    public interface IRoomFactory {
        public Room CreateRoom(IRoomSearchParams<Room> searchParams);
        public Room CreateRoomForParty<TEntity>(Party<Room> party, out TEntity rawTransactions)
            where TEntity : IEnumerable<RawTransaction>;
    }
}
