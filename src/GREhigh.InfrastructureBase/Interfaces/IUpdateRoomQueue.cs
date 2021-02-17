using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IUpdateRoomQueue {
        public bool Enqueue(UpdateQueueRecord updateRoom);
        public UpdateQueueRecord Dequeue();
    }
}
