using System;

namespace GREhigh.DomainBase.Interfaces {
    public class IUpdateRoom {
        public long RoomId { get; set; }
        public Type RoomType { get; set; }
    }
}
