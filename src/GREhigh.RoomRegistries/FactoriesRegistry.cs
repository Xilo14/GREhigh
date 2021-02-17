using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.RoomStaffBase.Interfaces;
using GREhigh.Utility;

namespace GREhigh.RoomRegistries {
    public class FactoriesRegistry : AbstractRegistry<IRoomFactory> {
        private FactoriesRegistry() { }
        public static FactoriesRegistry Instance {
            get { return Singleton<FactoriesRegistry>.Instance; }
            private set { }
        }
    }
}
