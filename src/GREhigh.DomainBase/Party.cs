using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;
using GREhigh.Utility.Interfaces;

namespace GREhigh.DomainBase {
    public class Party : IHaveId<ulong> {
        public virtual ulong PartyId { get; set; }
        public ulong Id { get => PartyId; set => PartyId = value; }
        public Type RoomType { get; set; }
        public List<Player> Players { get; set; }
        public IRoomSearchParams SearchParams { get; set; }
    }
    public class Party<TEntity> : Party where TEntity : Room {
        public Party() {
            RoomType = typeof(TEntity);
        }
    }
}
