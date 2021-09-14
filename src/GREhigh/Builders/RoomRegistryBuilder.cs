using System;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomRegistries;
using GREhigh.RoomStaffBase.Interfaces;

namespace GREhigh.Builders {
    public class RoomRegistryBuilder {
        private readonly HandlersRegistry _handlersRegistry;
        private readonly FactoriesRegistry _factoriesRegistry;
        private readonly RepositoriesRegistry _repositoriesRegistry;

        private IInfrastructureFactory<IRepository<Room>> _repositoryFactory;
        private IInfrastructureFactory<IRoomHandler<Room>> _handlerFactory;
        private IRoomFactory _roomFactory;
        private readonly GREhighClusterBuilder _builder;
        private readonly Type _roomType;

        internal RoomRegistryBuilder(
                HandlersRegistry handlersRegistry,
                FactoriesRegistry factoriesRegistry,
                RepositoriesRegistry repositoriesRegistry,
                GREhighClusterBuilder builder,
                Type roomType) {
            _handlersRegistry = handlersRegistry;
            _factoriesRegistry = factoriesRegistry;
            _repositoriesRegistry = repositoriesRegistry;

            _roomType = roomType;

            _builder = builder;
        }

        public RoomRegistryBuilder WithRepository(IInfrastructureFactory<IRepository<Room>> factory) {
            _repositoryFactory = factory;
            return this;
        }
        public RoomRegistryBuilder WithHandler(IInfrastructureFactory<IRoomHandler<Room>> factory) {
            _handlerFactory = factory;
            return this;
        }
        public RoomRegistryBuilder WithRoomFactory(IRoomFactory factory) {
            _roomFactory = factory;
            return this;
        }

        public GREhighClusterBuilder SaveRoomToRegistry() {
            _handlersRegistry.AddToRegistry(_roomType, _handlerFactory);
            _repositoriesRegistry.AddToRegistry(_roomType, _repositoryFactory);
            _factoriesRegistry.AddToRegistry(_roomType, _roomFactory);


            return _builder;
        }
    }
}
