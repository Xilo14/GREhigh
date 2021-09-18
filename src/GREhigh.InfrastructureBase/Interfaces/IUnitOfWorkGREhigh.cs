using System;
using System.Collections.Concurrent;
using System.Reflection;
using GREhigh.DomainBase;
using GREhigh.RoomRegistries;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IUnitOfWorkGREhigh : IDisposable {
        public void Save();
        public abstract bool TryGetRoomRepository<T1, T2>(Type typeRoom, out T1 repository)
            where T1 : IRepository<T2>
            where T2 : Room;
        public bool SetRepositoryRegistry<T>(T repositoryRegistry)
            where T : AbstractRegistry<IInfrastructureFactory<IRepository<Room>>>;
        public IRepository<Transaction> GetTransactionsRepository();
        public IRepository<Player> GetPlayerRepository();
    }
}
