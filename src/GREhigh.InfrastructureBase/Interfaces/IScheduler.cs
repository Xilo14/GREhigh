using System;
using GREhigh.DomainBase.Interfaces;
using GREhigh.InfrastructureBase.Interfaces;

namespace GREhigh.Infrastructure.Interfaces {
    public interface IScheduler {
        public void RemoveJob(object jobId);
        public object AddJobCancellation(TimeSpan timeout, object roomId);
        public object AddJobFinishPreparing(TimeSpan timeout, object roomId);
        public object AddJobTick(TimeSpan timeout, object roomId);
        public void SetUpdateProducer(IProducer<IUpdateRoom> producer);
        public delegate void JobDelegate(object RoomId);
    }
}
