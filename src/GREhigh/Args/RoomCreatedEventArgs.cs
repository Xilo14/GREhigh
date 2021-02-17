using System;
using GREhigh.DomainBase;

namespace GREhigh.Args {
    public class RoomCreatedEventArgs {
        public Room Room { get; }
        public DateTime DateTime { get; }
        public RoomCreatedEventArgs(Room room) {
            Room = room;
            DateTime = DateTime.UtcNow;
        }
    }
}
