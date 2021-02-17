using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;
using GREhigh.Infrastructure.Interfaces;

namespace GREhigh {
    public class UpdateRoomProducer {
        private readonly IUpdateRoomQueue _queue;
        internal UpdateRoomProducer(IUpdateRoomQueue queue) {
            _queue = queue;
        }
        public bool TryProduceUpdate(IUpdateRoom update) {
            return _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Update,
                UpdateRoom = update,
                RoomId = update.RoomId,
            });
        }
        internal void ProduceCancellation(object roomId) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Cancellation,
                RoomId = roomId,
            });
        }
        internal void ProduceTick(object roomId) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Tick,
                RoomId = roomId,
            });
        }
        internal void ProduceFinishPreparing(object roomId) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.FinishPreparing,
                RoomId = roomId,
            });
        }
    }
}

