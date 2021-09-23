using System;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.DomainBase {
    public class UpdateQueueRecord {
        public long RoomId { get; set; }
        public Type _roomType;
        public Type RoomType {
            get => _roomType; set {
                if (value.Namespace == "Castle.Proxies")
                    _roomType = value.BaseType;
                else
                    _roomType = value;
            }
        }
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
