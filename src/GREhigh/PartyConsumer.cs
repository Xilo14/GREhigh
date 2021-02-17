using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomRegistries;
using GREhigh.RoomStaffBase.Interfaces;
using GREhigh.Args;

namespace GREhigh {
    public class PartyConsumer {
        private readonly GREhighCluster _cluster;
        private readonly IPartyQueue _queue;
        private readonly IRoomSynchronizer _synchronizer;
        private readonly IInfrastructureFactory<IUnitOfWorkGREhigh> _uofFactory;
        private readonly FactoriesRegistry _factoriesRegistry;
        private readonly HandlersRegistry _handlersRegistry;
        private readonly ITransactionChef _transactionChef;
        private readonly IInfrastructureFactory<ISchedular> _schedularFactory;

        internal PartyConsumer(
                GREhighCluster cluster,
                IPartyQueue queue,
                IRoomSynchronizer synchronizer,
                IInfrastructureFactory<IUnitOfWorkGREhigh> uofFactory,
                FactoriesRegistry factoriesRegistry,
                HandlersRegistry handlersRegistry,
                ITransactionChef transactionChef,
                IInfrastructureFactory<ISchedular> schedularFactory) {
            _cluster = cluster;
            _queue = queue;
            _synchronizer = synchronizer;
            _uofFactory = uofFactory;
            _factoriesRegistry = factoriesRegistry;
            _transactionChef = transactionChef;
            _handlersRegistry = handlersRegistry;
            _schedularFactory = schedularFactory;
        }
        public async Task Consume() {
            while (true) {
                var party = _queue.Dequeue();
                if (party == null) {
                    await Task.Delay(100);
                    continue;
                }

                var uof = _uofFactory.GetInfrastructure();
                uof.SetRepositoryRegistry(_cluster.RepositoriesRegistry);
                if (!uof.TryGetRoomRepository(party.RoomType, out IRoomRepository<Room> roomRepository))
                    throw new Exception("Room was not registered!");//TODO exception

                var room = roomRepository.GetForParty(party).FirstOrDefault();

                if (room == null) {
                    _CreateRoom(party, uof, roomRepository);
                } else {
                    _AddToExistsRoom(room, party, uof, roomRepository);
                }

            }
        }

        private void _CreateRoom(
                Party<Room> party,
                IUnitOfWorkGREhigh uof,
                IRoomRepository<Room> roomRepository) {
            var rawTransactions = new List<RawTransaction>();
            var roomFactory = _factoriesRegistry.GetForRoom(party.RoomType);
            var room = roomFactory.CreateRoomForParty(party, out rawTransactions);
            var schedular = _schedularFactory.GetInfrastructure();


            var typeUpdate = RoomUpdatedEventArgs.UpdateTypeEnum.Created;

            var transactions = _transactionChef.Cook(rawTransactions);
            var transactionsRepository = uof.GetTransactionsRepository();
            transactionsRepository.Insert(transactions);

            uof.Save();
            if (room.IsCanStart && room.SchedularJobId == null) {
                room.SchedularJobId = schedular.AddJob(
                    _cluster.GetUpdateProducer().ProduceFinishPreparing,
                    room.PreparingTime,
                    room.RoomId);
            } else {
                room.SchedularJobId = schedular.AddJob(
                    _cluster.GetUpdateProducer().ProduceCancellation,
                    room.WaitingTimeout,
                    room.RoomId);
            }


            roomRepository.Insert(room);
            uof.Dispose();
            _cluster.OnRoomUpdated(
                                new RoomUpdatedEventArgs(
                                    room,
                                    typeUpdate));
        }
        private void _AddToExistsRoom(
                Room room,
                Party<Room> party,
                IUnitOfWorkGREhigh uof,
                IRoomRepository<Room> roomRepository) {
            if (!_synchronizer.TryLock(room)) {
                _queue.Enqueue(party);
                return;
            }
            var rawTransactions = new List<RawTransaction>();
            var handlerFactory = _handlersRegistry.GetForRoom(party.RoomType);
            var handler = handlerFactory.GetInfrastructure();
            handler.Room = room;

            if (!handler.TryAddParty(party, out rawTransactions)) {
                _synchronizer.Free(room);
                _queue.Enqueue(party);
                return;
            }
            var typeUpdate = RoomUpdatedEventArgs.UpdateTypeEnum.UsersAdded;
            var schedular = _schedularFactory.GetInfrastructure();
            var transactions = _transactionChef.Cook(rawTransactions);
            var transactionsRepository = uof.GetTransactionsRepository();
            transactionsRepository.Insert(transactions);

            if (room.IsCanStart && handler.IsStateChanged) {
                schedular.RemoveJob(room.SchedularJobId);
                room.SchedularJobId = schedular.AddJob(
                    _cluster.GetUpdateProducer().ProduceFinishPreparing,
                    room.PreparingTime,
                    room.RoomId);
            }

            roomRepository.Update(room);
            uof.Save();
            uof.Dispose();
            _cluster.OnRoomUpdated(
                                new RoomUpdatedEventArgs(
                                    room,
                                    typeUpdate));
            _synchronizer.Free(room);
        }
    }
}
