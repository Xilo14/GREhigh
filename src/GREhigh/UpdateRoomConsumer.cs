using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly ITransactionChef<Transaction> _transactionChef;
        private readonly IInfrastructureFactory<IScheduler> _schedulerFactory;
        internal UpdateRoomConsumer(
                GREhighCluster cluster,
                IUpdateRoomQueue queue,
                IRoomSynchronizer synchronizer,
                IInfrastructureFactory<IUnitOfWorkGREhigh> uofFactory,
                FactoriesRegistry factoriesRegistry,
                HandlersRegistry handlersRegistry,
                ITransactionChef<Transaction> transactionChef,
                IInfrastructureFactory<IScheduler> schedulerFactory) {
            _cluster = cluster;
            _queue = queue;
            _synchronizer = synchronizer;
            _uofFactory = uofFactory;
            _factoriesRegistry = factoriesRegistry;
            _transactionChef = transactionChef;
            _handlersRegistry = handlersRegistry;
            _schedulerFactory = schedulerFactory;
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
                    _ConsumeFromScheduler(record);
                }
            }
        }
        private void _ConsumeFromScheduler(UpdateQueueRecord record) {
            var uof = _uofFactory.GetInfrastructure();
            uof.SetRepositoryRegistry(_cluster.RepositoriesRegistry);
            if (!uof.TryGetRoomRepository<IRepository<Room>, Room>(record.RoomType, out IRepository<Room> roomRepository))
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
            handler.Randomizer = _cluster._params.RandomizerFactory.GetInfrastructure();

            var scheduler = _schedulerFactory.GetInfrastructure();
            scheduler.SetUpdateProducer(_cluster.GetUpdateProducer());
            switch (record.RecordType) {
                case UpdateQueueRecord.RecordTypeEnum.Cancellation:
                    if (room.SchedulerJobId != null)
                        scheduler.RemoveJob(room.SchedulerJobId);
                    if (room.Status == Room.StatusEnum.WaitingForUser)
                        handler.Cancel(out rawTransactions);
                    break;
                case UpdateQueueRecord.RecordTypeEnum.FinishPreparing:
                    handler.Start(out rawTransactions);
                    if (room.Status == Room.StatusEnum.InProccess
                    && room.TickInterval != Timeout.InfiniteTimeSpan)
                        room.SchedulerJobId = scheduler.AddJobTick(
                                room.TickInterval,
                                room.RoomId,
                                room.GetType());
                    break;
                case UpdateQueueRecord.RecordTypeEnum.Tick:
                    handler.Tick(out rawTransactions);
                    if (room.Status == Room.StatusEnum.InProccess
                    && room.TickInterval != Timeout.InfiniteTimeSpan)
                        room.SchedulerJobId = scheduler.AddJobTick(
                                room.TickInterval,
                                room.RoomId,
                                room.GetType());
                    break;
            }

            var transactions = _transactionChef.Cook(rawTransactions);
            var transactionsRepository = uof.GetTransactionsRepository();
            transactionsRepository.Insert(transactions);
            room.Transactions.AddRange(transactions);
            _ = room.Players;
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
            if (!uof.TryGetRoomRepository<IRepository<Room>, Room>(update.RoomType, out IRepository<Room> roomRepository))
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
            handler.Randomizer = _cluster._params.RandomizerFactory.GetInfrastructure();

            if (!handler.HandleUpdate(update, out rawTransactions)) {

                _queue.Enqueue(record);
                return;
            }
            var transactionsRepository = uof.GetTransactionsRepository();
            var transactions = _transactionChef.Cook(rawTransactions);

            foreach (var group in transactions.GroupBy(t => t.Player)) {
                var currentAmount = transactionsRepository.Where(x => x.Player.Id == group.Key.Id).Sum(x => x.Amount);
                var dif = group.Sum(t => t.Amount);
                if (currentAmount + dif < 0) {
                    _synchronizer.Free(room);
                    return;
                }
            }


            var scheduler = _schedulerFactory.GetInfrastructure();
            scheduler.SetUpdateProducer(_cluster.GetUpdateProducer());
            if (room.IsCanStart && handler.IsStateChanged) {
                scheduler.RemoveJob(room.SchedulerJobId);
                room.SchedulerJobId = scheduler.AddJobFinishPreparing(
                    room.PreparingTime,
                    room.RoomId,
                    room.GetType());
            }

            transactionsRepository.Insert(transactions);
            room.Transactions.AddRange(transactions);
            _ = room.Players;
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

