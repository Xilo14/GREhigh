using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.DomainBase {
    public class Party {
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
