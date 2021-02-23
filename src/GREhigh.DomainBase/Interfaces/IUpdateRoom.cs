using System;

namespace GREhigh.DomainBase.Interfaces {
    public class IUpdateRoom {
        public ulong RoomId { get; set; }
        public Type RoomType { get; set; }
    }
}
