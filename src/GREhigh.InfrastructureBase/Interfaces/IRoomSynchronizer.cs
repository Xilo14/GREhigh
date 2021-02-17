using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IRoomSynchronizer {
        public bool TryLock(Room room);
        public void Free(Room room);
    }
}
