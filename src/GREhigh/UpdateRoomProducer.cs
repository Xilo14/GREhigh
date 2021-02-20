using System;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.InfrastructureBase.Interfaces;

namespace GREhigh {
    public class UpdateRoomProducer : IProducer<IUpdateRoom> {
        private readonly IUpdateRoomQueue _queue;
        internal UpdateRoomProducer(IUpdateRoomQueue queue) {
            _queue = queue;
        }
        public bool TryProduce(IUpdateRoom update) {
            return _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Update,
                UpdateRoom = update,
                RoomId = update.RoomId,
            });
        }
        public void ProduceCancellation(object roomId, Type roomType) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Cancellation,
                RoomId = roomId,
                RoomType = roomType
            });
        }
        public void ProduceTick(object roomId, Type roomType) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.Tick,
                RoomId = roomId,
                RoomType = roomType
            });
        }
        public void ProduceFinishPreparing(object roomId, Type roomType) {
            _queue.Enqueue(new UpdateQueueRecord() {
                RecordType = UpdateQueueRecord.RecordTypeEnum.FinishPreparing,
                RoomId = roomId,
                RoomType = roomType
            });
        }
    }
}

