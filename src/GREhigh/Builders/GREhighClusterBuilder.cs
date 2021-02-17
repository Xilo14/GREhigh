using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomRegistries;

namespace GREhigh.Builders {
    public class GREhighClusterBuilder {
        private readonly GREhighCluster.ClusterParams _params = new();
        public GREhighClusterBuilder() {

        }
        public GREhighClusterBuilder WithRandomizer(IInfrastructureFactory<IRandomizer> factory) {
            _params.RandomizerFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithPartyQueue(IInfrastructureFactory<IPartyQueue> factory) {
            _params.PartyQueueFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithUpdateRoomQueue(IInfrastructureFactory<IUpdateRoomQueue> factory) {
            _params.UpdateRoomQueueFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithSchedular(IInfrastructureFactory<ISchedular> factory) {
            _params.SchedularFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithTransactionChef(IInfrastructureFactory<ITransactionChef> factory) {
            _params.TransactionChefFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithUnitOfWork(IInfrastructureFactory<IUnitOfWorkGREhigh> factory) {
            _params.UnitOfWorkFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithRoomSynchronizer(IInfrastructureFactory<IRoomSynchronizer> factory) {
            _params.RoomSynchronizerFactory = factory;
            return this;
        }
        public GREhighClusterBuilder WithCountPartyConsumer(uint count) {
            _params.CountPartyConsumerThreads = count;
            return this;
        }
        public GREhighClusterBuilder WithCountUpdateRoomConsumer(uint count) {
            _params.CountUpdateConsumerThreads = count;
            return this;
        }

        public RoomRegistryBuilder AddRoomToRegistry<TEntity>() {
            return new RoomRegistryBuilder(
                HandlersRegistry.Instance,
                FactoriesRegistry.Instance,
                RepositoriesRegistry.Instance,
                this,
                typeof(TEntity)
            );
        }
    }
}
