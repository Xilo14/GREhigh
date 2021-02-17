using System;

namespace GREhigh.Infrastructure.Interfaces {
    public interface ISchedular {
        public void RemoveJob(object jobId);
        public object AddJob(JobDelegate job, TimeSpan timeout, object roomId);

        public delegate void JobDelegate(object RoomId);
    }
}
