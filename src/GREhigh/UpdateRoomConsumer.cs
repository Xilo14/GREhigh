using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GREhigh.DomainBase;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.RoomRegistries;
using GREhigh.RoomStaffBase.Interfaces;

namespace GREhigh {
    public class UpdateRoomConsumer {
        private readonly GREhighCluster _cluster;
        private readonly IUpdateRoomQueue _queue;
        private readonly IRoomSynchronizer _synchronizer;
        private readonly IInfrastructureFactory<IUnitOfWorkGREhigh> _uofFactory;
        private readonly FactoriesRegistry _factoriesRegistry;
        private readonly HandlersRegistry _handlersRegistry;
        private readonly ITransactionChef _transactionChef;
        private readonly IInfrastructureFactory<ISchedular> _schedularFactory;
        internal UpdateRoomConsumer(
                GREhighCluster cluster,
                IUpdateRoomQueue queue,
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
        public async void Consume() {
            while (true) {
                var record = _queue.Dequeue();

                if (record == null) {
                    await Task.Delay(100);
                    continue;
                }

                if (record.RecordType == UpdateQueueRecord.RecordTypeEnum.Update) {
                    _ConsumeUpdate(record);
                } else {
                    _ConsumeFromSchedular(record);
                }





            }
        }

        private void _ConsumeFromSchedular(UpdateQueueRecord record) {
            var uof = _uofFactory.GetInfrastructure();
            uof.SetRepositoryRegistry(_cluster.RepositoriesRegistry);
            if (!uof.GetRepository(record.RoomType, out IRoomRepository<Room> roomRepository))
                throw new Exception("Room was not registered!");//TODO exception

            var room = roomRepository.GetByID(record.RoomId);

            var rawTransactions = new List<RawTransaction>();

            if (room == null) {
                throw new Exception("Room was not found!");//TODO exception
            }

            if (!_synchronizer.TryLock(room)) {
                _queue.Enqueue(record);
                return;
            }
            var handlerFactory = _handlersRegistry.GetForRoom(record.RoomType);
            var handler = handlerFactory.GetInfrastructure();
            handler.Room = room;

            var schedular = _schedularFactory.GetInfrastructure();
            switch (record.RecordType) {
                case UpdateQueueRecord.RecordTypeEnum.Cancellation:
                    if (room.SchedularJobId != null)
                        schedular.RemoveJob(room.SchedularJobId);
                    handler.Cancel(out rawTransactions);
                    break;
                case UpdateQueueRecord.RecordTypeEnum.FinishPreparing:
                    handler.Start(out rawTransactions);
                    room.SchedularJobId = schedular.AddJob(
                            _cluster.GetUpdateProducer().ProduceTick,
                            room.TickInterval,
                            room.RoomId);
                    break;
                case UpdateQueueRecord.RecordTypeEnum.Tick:
                    handler.Tick(out rawTransactions);
                    room.SchedularJobId = schedular.AddJob(
                            _cluster.GetUpdateProducer().ProduceTick,
                            room.TickInterval,
                            room.RoomId);
                    break;
            }



            var transactions = _transactionChef.Cook(rawTransactions);
            uof.GetRepository(out IRepository<Transaction> transactionsRepository);
            transactionsRepository.Insert(transactions);

            uof.Save();
            uof.Dispose();
            _cluster.OnRoomUpdated(
                new Args.RoomUpdatedEventArgs(
                    room,
                    Args.RoomUpdatedEventArgs.UpdateTypeEnum.Updated));
            _synchronizer.Free(room);
        }

        private void _ConsumeUpdate(UpdateQueueRecord record) {
            var update = record.UpdateRoom;
            var uof = _uofFactory.GetInfrastructure();
            uof.SetRepositoryRegistry(_cluster.RepositoriesRegistry);
            if (!uof.GetRepository(update.RoomType, out IRoomRepository<Room> roomRepository))
                throw new Exception("Room was not registered!");//TODO exception

            var room = roomRepository.GetByID(update.RoomId);

            var rawTransactions = new List<RawTransaction>();

            if (room == null) {
                throw new Exception("Room was not found!");//TODO exception
            }

            if (!_synchronizer.TryLock(room)) {
                _queue.Enqueue(record);
                return;
            }
            var handlerFactory = _handlersRegistry.GetForRoom(update.RoomType);
            var handler = handlerFactory.GetInfrastructure();
            handler.Room = room;

            if (!handler.HandleUpdate(update, out rawTransactions)) {
                _synchronizer.Free(room);
                _queue.Enqueue(record);
                return;
            }
            var schedular = _schedularFactory.GetInfrastructure();
            if (room.IsCanStart && handler.IsStateChanged) {
                schedular.RemoveJob(room.SchedularJobId);
                room.SchedularJobId = schedular.AddJob(
                    _cluster.GetUpdateProducer().ProduceFinishPreparing,
                    room.PreparingTime,
                    room.RoomId);
            }

            var transactions = _transactionChef.Cook(rawTransactions);
            uof.GetRepository(out IRepository<Transaction> transactionsRepository);
            transactionsRepository.Insert(transactions);

            uof.Save();
            uof.Dispose();
            _cluster.OnRoomUpdated(
                new Args.RoomUpdatedEventArgs(
                    room,
                    Args.RoomUpdatedEventArgs.UpdateTypeEnum.Updated));
            _synchronizer.Free(room);
        }
    }
}

