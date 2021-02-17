using System;

namespace GREhigh.DomainBase.Interfaces {
    public class IUpdateRoom {
        public object RoomId { get; set; }
        public Type RoomType { get; set; }
    }
}
