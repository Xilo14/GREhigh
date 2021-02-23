using System.Collections.Generic;
using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IPartyQueue {
        public bool Enqueue<TEntity>(TEntity party) where TEntity : Party;
        public bool IsPartyUniq<TEntity>(TEntity party) where TEntity : Party;
        public Party Dequeue();

    }
}
