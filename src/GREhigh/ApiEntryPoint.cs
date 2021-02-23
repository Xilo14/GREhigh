using System.Linq;
using GREhigh.DomainBase;
using static GREhigh.GREhighCluster;

namespace GREhigh {
    public class ApiEntryPoint {
        private ClusterParams _clusterParams;
        internal ApiEntryPoint(ClusterParams clusterParams) {
            _clusterParams = clusterParams;
        }
        public Player CreateNewPlayer() {
            var uof = _clusterParams.UnitOfWorkFactory.GetInfrastructure();
            var pRep = uof.GetPlayerRepository();
            var p = new Player();
            pRep.Insert(p);
            uof.Save();
            uof.Dispose();
            return p;
        }
        public int GetAmountCoins(Player player) {
            var uof = _clusterParams.UnitOfWorkFactory.GetInfrastructure();
            var transRep = uof.GetTransactionsRepository();
            return transRep.Where(x => x.Player == player).Sum(x => x.Amount);
        }

        public bool AddCoins(uint amount, Player player) {
            var uof = _clusterParams.UnitOfWorkFactory.GetInfrastructure();
            var transRep = uof.GetTransactionsRepository();
            var rawTrans = new RawTransaction() {
                Player = player,
                Type = Transaction.TransactionTypeEnum.Deposit,
                Amount = (int)amount,
            };
            var trans = _clusterParams.TransactionChefFactory.GetInfrastructure().Cook(rawTrans);
            transRep.Insert(trans);
            uof.Save();
            uof.Dispose();
            return true;
        }
        public bool WithdrawCoins(uint amount, Player player) {
            var uof = _clusterParams.UnitOfWorkFactory.GetInfrastructure();
            var transRep = uof.GetTransactionsRepository();
            var currentAmount = transRep.Get(x => x.Player == player).Sum(x => x.Amount);
            if (amount > currentAmount)
                return false;
            var rawTrans = new RawTransaction() {
                Player = player,
                Type = Transaction.TransactionTypeEnum.Deposit,
                Amount = -(int)amount,
            };
            var trans = _clusterParams.TransactionChefFactory.GetInfrastructure().Cook(rawTrans);
            transRep.Insert(trans);
            uof.Save();
            uof.Dispose();
            return true;
        }
    }
}
