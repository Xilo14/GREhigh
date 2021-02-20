using System;

namespace GREhigh.DomainBase {
    public class Transaction {
        public Player Player { get; set; }
        public int Amount { get; set; }
        public TransactionTypeEnum Type { get; set; }
        public object AdditionalInformation { get; set; }
        public enum TransactionTypeEnum {
            Result,
            Freeze,
            Deposit,
            Transfer
        }
    }
}
