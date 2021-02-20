using System;
using System.Collections.Concurrent;
using System.Reflection;
using GREhigh.DomainBase;
using GREhigh.RoomRegistries;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IUnitOfWorkGREhigh : IDisposable {
        public void Save();
        public bool TryGetRoomRepository<T>(Type typeRoom, out T repository) where T : IRepository<Room>;
        public bool SetRepositoryRegistry<T>(T repositoryRegistry)
            where T : AbstractRegistry<IInfrastructureFactory<IRoomRepository>>;
        public IRepository<Transaction> GetTransactionsRepository();
        public IRepository<Player> GetPlayerRepository();
    }
}
