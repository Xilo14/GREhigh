using System;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.DomainBase.Interfaces;
using GREhigh.Infrastructure.Interfaces;

namespace GREhigh.RoomStaffBase.Interfaces {
    public interface IRoomHandler<TRoom> where TRoom : Room {
        public Room Room { get; set; }
        public IRandomizer Randomizer { get; set; }
        public bool IsStateChanged { get; set; }
        public bool HandleUpdate<TEnumerableRawTransaction>(
            IUpdateRoom updateRoom,
            out TEnumerableRawTransaction rawTransactions)
                where TEnumerableRawTransaction : IEnumerable<RawTransaction>;
        public bool TryAddParty<TEnumerableRawTransaction>(
            Party party,
            out TEnumerableRawTransaction rawTransactions)
                where TEnumerableRawTransaction : IEnumerable<RawTransaction>, new();
        public void Cancel<TEnumerableRawTransaction>(
            out TEnumerableRawTransaction rawTransactions)
                where TEnumerableRawTransaction : IEnumerable<RawTransaction>;
        public void Tick<TEnumerableRawTransaction>(
            out TEnumerableRawTransaction rawTransactions)
                where TEnumerableRawTransaction : IEnumerable<RawTransaction>;
        public void Start<TEnumerableRawTransaction>(
            out TEnumerableRawTransaction rawTransactions)
                where TEnumerableRawTransaction : IEnumerable<RawTransaction>;
    }
}
