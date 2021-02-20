using System;
using System.Collections.Generic;

namespace GREhigh.DomainBase {
    public abstract class Room {
        public virtual object Result { get; set; }
        public virtual List<Player> Players { get; set; }
        public virtual object RoomId { get; set; }
        public virtual object SchedulerJobId { get; set; }
        public virtual bool IsCanStart { get; set; }
        public virtual StatusEnum Status { get; set; }
        public TimeSpan WaitingTimeout = TimeSpan.FromHours(3);
        public TimeSpan PreparingTime = TimeSpan.FromSeconds(15);
        public TimeSpan TickInterval = TimeSpan.FromTicks(0);

        public enum StatusEnum {
            WaitingForUser,
            PreparingForGame,
            Finished,
            InProccess,
            Canceled
        }
    }
}
