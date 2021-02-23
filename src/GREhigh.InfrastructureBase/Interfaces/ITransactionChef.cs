using System.Collections.Generic;
using GREhigh.DomainBase;

namespace GREhigh.Infrastructure.Interfaces {
    public interface ITransactionChef {
        public IEnumerable<Transaction> Cook(IEnumerable<RawTransaction> rawTransactions);
        public Transaction Cook(RawTransaction rawTransaction);
    }
}
