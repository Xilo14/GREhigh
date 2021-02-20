using System;
using System.Collections.Generic;
using GREhigh;
using GREhigh.Builders;
using GREhigh.DomainBase;
using GREhigh.Games.HeadsOrTails;
using GREhigh.Infrastructure.DataStorageInMemory;
using GREhigh.Infrastructure.Interfaces;
using GREhigh.Infrastructure.PartyQueueInMemory;
using GREhigh.Infrastructure.QuartzScheduler;
using GREhigh.Infrastructure.RoomSynchronizerInMemory;
using GREhigh.Infrastructure.UpdateRoomQueueInMemory;
using GREhigh.Infratructure.SimpleRandomizer;

namespace ConsoleSimpleExample {
    class Program {
        static void Main(string[] args) {
            var clusterBuilder = new GREhighClusterBuilder();
            var cluster = clusterBuilder
                .WithUnitOfWork(new UnitOfWorkInMemoryFactory())
                .WithUpdateRoomQueue(new UpdateRoomQueueInMemoryFactory())
                .WithPartyQueue(new PartyQueueInMemoryFactory())
                .WithTransactionChef(new TransactionChefMoqFactory())
                .WithRoomSynchronizer(new RoomSynchronizerInMemoryFactory())
                .WithRandomizer(new SimpleRandomizerFactory())
                .WithScheduler(new QuartzSchedulerFactory())
                .WithCountPartyConsumer(1)
                .WithCountUpdateRoomConsumer(1)
                .AddRoomToRegistry<HoTRoom>()
                    .WithHandler(new HoTRoomHandlerFactory())
                    .WithRepository(new HoTRoomRepositoryFactory())
                    .WithRoomFactory(new HoTRoomFactory())
                    .SaveRoomToRegistry()
                .Build();


            cluster.RoomUpdated += (sender, args) => {
                Console.WriteLine(args.UpdateType.ToString() + $" {args.Room.RoomId} {args.Room.Status}");
                if (args.Room.Status == Room.StatusEnum.Finished) {
                    Console.WriteLine("w:" + ((HoTRoomResult)args.Room.Result).winner.PlayerId);
                    Console.WriteLine("l:" + ((HoTRoomResult)args.Room.Result).loser.PlayerId);
                }
            };
            cluster.Start();
            Console.ReadLine();

            var party = new HoTParty() {
                Players = new List<Player>() { new Player() { PlayerId = 1 } },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            Console.ReadLine();

            party = new HoTParty() {
                Players = new List<Player>() { new Player() { PlayerId = 2 } },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);

            party = new HoTParty() {
                Players = new List<Player>() { new Player() { PlayerId = 3 }, new Player() { PlayerId = 4 } },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            Console.ReadLine();

            party = new HoTParty() {
                Players = new List<Player>() { new Player() { PlayerId = 5 } },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            Console.ReadLine();
        }

        public class TransactionChefMoq : ITransactionChef {
            public IEnumerable<Transaction> Cook(IEnumerable<RawTransaction> rawTransactions) {
                List<Transaction> coocked = new();
                foreach (var item in rawTransactions) {
                    coocked.Add(item);
                };
                return coocked;
            }
        }
        public class TransactionChefMoqFactory : IInfrastructureFactory<ITransactionChef> {
            public ITransactionChef GetInfrastructure() {
                return new TransactionChefMoq();
            }
        }
    }
}
