using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.InfrastructureBase.Interfaces;

namespace GREhigh {
    public class PartyProducer : IProducer<Party> {
        private readonly IPartyQueue _queue;
        internal PartyProducer(
                IPartyQueue queue) {
            _queue = queue;
        }

        public bool TryProduce(Party party) {
            if (!_queue.IsPartyUniq(party))
                return false;
            return _queue.Enqueue(party);
        }
    }
}
