using System;
using System.Collections.Generic;
using GREhigh.Utility.Interfaces;

namespace GREhigh.DomainBase {
    public class Player : IHaveId<long> {
        public long Id { get; set; }
        public virtual List<Room> Rooms { get; set; }
    }
}
