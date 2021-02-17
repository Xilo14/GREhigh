using System;
using System.Collections.Concurrent;
using System.Reflection;
using GREhigh.DomainBase;
using GREhigh.RoomRegistries;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IUnitOfWorkGREhigh : IDisposable {
        public void Save();
        public bool GetRepository<TEntity>(out TEntity repository) where TEntity : IRepository;
        public bool GetRepository<TEntity>(Type typeRoom, out TEntity repository) where TEntity : IRepository;
        public bool SetRepositoryRegistry<TEntity>(AbstractRegistry<TEntity> repositoryRegistry)
            where TEntity : class;
    }
}
