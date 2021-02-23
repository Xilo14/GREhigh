using System;
using GREhigh.Utility.Interfaces;

namespace GREhigh.DomainBase {
    public class Player : IHaveId<ulong> {
        public ulong PlayerId { get; set; }
        public ulong Id { get => PlayerId; set => PlayerId = value; }
    }
}
