using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;

namespace GREhigh.RoomRegistries {
    public abstract class AbstractRegistry<TEntity> : IRegistry<TEntity> where TEntity : class {
        private readonly ConcurrentDictionary<Type, TEntity> _registry = new();
        public TEntity GetForRoom(Type roomType)
            => _registry.GetValueOrDefault(roomType, null);
        public TEntity GetForRoom(Room room)
            => GetForRoom(room.GetType());
        public bool AddToRegistry(Type key, TEntity value)
            => _registry.TryAdd(key, value);
    }
}
