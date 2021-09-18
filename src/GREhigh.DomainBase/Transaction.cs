using System;
using GREhigh.Utility.Interfaces;

namespace GREhigh.DomainBase {
    public class Transaction : IHaveId<ulong> {
        public ulong TransactionId { get; set; }
        public ulong Id { get => TransactionId; set => TransactionId = value; }
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
