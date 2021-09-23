using System;
using GREhigh.Utility.Interfaces;

namespace GREhigh.DomainBase {
    public class Transaction : IHaveId<long> {
        public long Id { get; set; }
        public virtual Player Player { get; set; }
        public int Amount { get; set; }
        public TransactionTypeEnum Type { get; set; }
        public enum TransactionTypeEnum {
            Result,
            Freeze,
            Deposit,
            Transfer
        }
    }
}
