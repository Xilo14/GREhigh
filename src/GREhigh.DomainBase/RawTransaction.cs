using System;

namespace GREhigh.DomainBase {
    public class RawTransaction : Transaction, IRawTransaction {
        public RawTransaction() { }
        public RawTransaction(Player player, int amount, TransactionTypeEnum type)
         => (Player, Amount, Type) = (player, amount, type);
    }
}
