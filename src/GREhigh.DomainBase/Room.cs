using System;

namespace GREhigh.DomainBase {
    public abstract class Room {
        public object RoomId { get; set; }
        public object SchedularJobId { get; set; }
        public bool IsCanStart { get; }
        public StatusEnum Status { get; set; }
        public readonly TimeSpan WaitingTimeout = TimeSpan.FromHours(3);
        public readonly TimeSpan PreparingTime = TimeSpan.FromSeconds(15);
        public readonly TimeSpan TickInterval = TimeSpan.FromTicks(0);

        public enum StatusEnum {
            WaitingForUser,
            PreparingForGame,
            Finished,
            InProccess,
            Canceled
        }
    }
}
