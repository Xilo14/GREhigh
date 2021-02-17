using System;
using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IRegistry<TEntity> {
        public TEntity GetForRoom(Type roomType);
        public TEntity GetForRoom(Room room);
        public bool AddToRegistry(Type key, TEntity value);
    }
}
