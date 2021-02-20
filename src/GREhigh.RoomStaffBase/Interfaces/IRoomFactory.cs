using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.RoomStaffBase.Interfaces {
    public interface IRoomFactory {
        public Room CreateRoom(IRoomSearchParams<Room> searchParams);
        public bool TryCreateRoomForParty<TEntity>(out Room room, Party party, out TEntity rawTransactions)
            where TEntity : IEnumerable<RawTransaction>;
    }
}
