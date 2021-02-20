using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomStaffBase.Interfaces;
using GREhigh.Utility;

namespace GREhigh.RoomRegistries {
    public class RepositoriesRegistry : AbstractRegistry<IInfrastructureFactory<IRoomRepository>> {
        private RepositoriesRegistry() { }
        public static RepositoriesRegistry Instance {
            get { return Singleton<RepositoriesRegistry>.Instance; }
            private set { }
        }
    }
}
