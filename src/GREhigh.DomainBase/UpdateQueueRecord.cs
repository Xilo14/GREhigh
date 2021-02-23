using System;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.DomainBase {
    public class UpdateQueueRecord {
        public ulong RoomId { get; set; }
        public Type RoomType { get; set; }
        public IUpdateRoom UpdateRoom { get; set; }
        public RecordTypeEnum RecordType { get; set; }
        public enum RecordTypeEnum {
            Update,
            Tick,
            Cancellation,
            FinishPreparing,
        }
    }
}
