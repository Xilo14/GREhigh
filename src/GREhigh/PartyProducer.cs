using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;

namespace GREhigh {
    public class PartyProducer {
        private readonly IPartyQueue _queue;
        internal PartyProducer(
                IPartyQueue queue) {
            _queue = queue;
        }
        public bool TryProduceParty<TPartyEntity>(TPartyEntity party)
                where TPartyEntity : Party<Room> {
            //TODO check coins
            return _queue.Enqueue(party);
        }
    }
}
