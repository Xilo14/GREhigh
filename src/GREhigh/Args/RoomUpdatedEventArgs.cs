using System;
using GREhigh.DomainBase;

namespace GREhigh.Args {
    public class RoomUpdatedEventArgs {
        public Room Room { get; }
        public DateTime DateTime { get; }
        public UpdateTypeEnum UpdateType { get; }

        public RoomUpdatedEventArgs(Room room, UpdateTypeEnum updateType) {
            Room = room;
            DateTime = DateTime.UtcNow;
            UpdateType = updateType;
        }
        public enum UpdateTypeEnum {
            UsersAdded,
            Updated,
            Created,
        }
    }
}
