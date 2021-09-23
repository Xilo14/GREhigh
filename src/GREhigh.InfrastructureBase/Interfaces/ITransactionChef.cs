using System.Collections.Generic;
using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface ITransactionChef<out T> where T : Transaction {
        public IEnumerable<T> Cook(IEnumerable<RawTransaction> rawTransactions);
        public T Cook(RawTransaction rawTransaction);
    }
}
