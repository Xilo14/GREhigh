using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.DomainBase {
    public class Party<TEntity> where TEntity : Room {
        public Type RoomType { get; } = typeof(TEntity);
        public List<Player> Players { get; set; }
        public IRoomSearchParams<TEntity> SearchParams { get; set; }
    }
}
