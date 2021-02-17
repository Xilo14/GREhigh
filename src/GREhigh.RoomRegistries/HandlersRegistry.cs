using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomStaffBase.Interfaces;
using GREhigh.Utility;

namespace GREhigh.RoomRegistries {
    public class HandlersRegistry : AbstractRegistry<IInfrastructureFactory<IRoomHandler<Room>>> {
        private HandlersRegistry() { }
        public static HandlersRegistry Instance {
            get { return Singleton<HandlersRegistry>.Instance; }
            private set { }
        }
    }
}
