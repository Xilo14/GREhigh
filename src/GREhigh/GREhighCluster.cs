using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using GREhigh.Args;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomRegistries;
using GREhigh.Utility;

namespace GREhigh {
    public class GREhighCluster {
        private ClusterStateEnum _clusterState;
        public ClusterStateEnum ClusterState {
            get => _clusterState;
            private set { _clusterState = value; throw new NotImplementedException(); }
        }

        private readonly List<Task> _partyConsumers;
        private readonly List<Task> _updateConsumers;
        internal readonly ClusterParams _params;
        public RepositoriesRegistry RepositoriesRegistry { get; private set; }
        public HandlersRegistry HandlersRegistry { get; private set; }
        public FactoriesRegistry FactoriesRegistry { get; private set; }

        internal GREhighCluster(ClusterParams clusterParams) {
            _params = clusterParams;
            _clusterState = ClusterStateEnum.Initialize;

            RepositoriesRegistry = RepositoriesRegistry.Instance;
            HandlersRegistry = HandlersRegistry.Instance;
            FactoriesRegistry = FactoriesRegistry.Instance;

            ClusterState = ClusterStateEnum.Ready;
        }
        internal class ClusterParams {
            public uint CountPartyConsumerThreads { get; set; } = 1;
            public uint CountUpdateConsumerThreads { get; set; } = 1;

            public IInfrastructureFactory<IPartyQueue> PartyQueueFactory { get; set; }
            public IInfrastructureFactory<IUpdateRoomQueue> UpdateRoomQueueFactory { get; set; }
            public IInfrastructureFactory<IRoomSynchronizer> RoomSynchronizerFactory { get; set; }
            public IInfrastructureFactory<ITransactionChef> TransactionChefFactory { get; set; }
            public IInfrastructureFactory<IRandomizer> RandomizerFactory { get; set; }
            public IInfrastructureFactory<ISchedular> SchedularFactory { get; set; }
            public IInfrastructureFactory<IUnitOfWorkGREhigh> UnitOfWorkFactory { get; set; }

        }
        public enum ClusterStateEnum {
            Initialize,
            Ready,
            Started,
            Paused,
            Stoped,
        }

        public PartyProducer GetPartyProducer()
            => new(_params.PartyQueueFactory.GetInfrastructure());
        public UpdateRoomProducer GetUpdateProducer()
            => new(_params.UpdateRoomQueueFactory.GetInfrastructure());


        public async Task<bool> StartAsync() {
            return await new Task<bool>(Start);
        }

        public bool Start() {
            if (ClusterState != ClusterStateEnum.Ready)
                throw new Exception();//TODO exception

            var cancellationSource = new CancellationTokenSource();

            for (var i = 0; i < _params.CountPartyConsumerThreads; i++) {
                _partyConsumers.Add(
                    Task.Factory.StartNew(
                    new PartyConsumer(
                        this,
                        _params.PartyQueueFactory.GetInfrastructure(),
                        _params.RoomSynchronizerFactory.GetInfrastructure(),
                        _params.UnitOfWorkFactory,
                        FactoriesRegistry,
                        HandlersRegistry,
                        _params.TransactionChefFactory.GetInfrastructure(),
                        _params.SchedularFactory
                        ).Consume,
                    cancellationSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default
                ));
            }

            for (var i = 0; i < _params.CountUpdateConsumerThreads; i++) {
                _partyConsumers.Add(
                    Task.Factory.StartNew(
                    new UpdateRoomConsumer(
                        this,
                        _params.UpdateRoomQueueFactory.GetInfrastructure(),
                        _params.RoomSynchronizerFactory.GetInfrastructure(),
                        _params.UnitOfWorkFactory,
                        FactoriesRegistry,
                        HandlersRegistry,
                        _params.TransactionChefFactory.GetInfrastructure(),
                        _params.SchedularFactory
                        ).Consume,
                    cancellationSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default
                ));
            }

            ClusterState = ClusterStateEnum.Started;
            return true;
        }
        public async Task<bool> PauseAsync() {
            throw new NotImplementedException();
        }
        public bool Pause() {
            throw new NotImplementedException();
        }
        public async Task<bool> StopAsync() {
            throw new NotImplementedException();
        }
        public bool Stop() {
            throw new NotImplementedException();
        }


        #region Events

        private EventHandlerList __Events = new();

        private object __evRoomUpdated = new();
        private object __evRoomCreated = new();

        public event EventHandler<RoomUpdatedEventArgs> RoomUpdated {
            add {
                __Events.AddHandler(__evRoomUpdated, value);
            }
            remove {
                __Events.RemoveHandler(__evRoomUpdated, value);
            }
        }
        public event EventHandler<RoomCreatedEventArgs> RoomCreated {
            add {
                __Events.AddHandler(__evRoomCreated, value);
            }
            remove {
                __Events.RemoveHandler(__evRoomCreated, value);
            }
        }

        internal void OnRoomUpdated(RoomUpdatedEventArgs e) {
            (__Events[__evRoomUpdated] as EventHandler<RoomUpdatedEventArgs>)?.Invoke(this, e);
        }
        internal void OnRoomCreated(RoomCreatedEventArgs e) {
            (__Events[__evRoomCreated] as EventHandler<RoomCreatedEventArgs>)?.Invoke(this, e);
        }


        #endregion Events
    }
}
