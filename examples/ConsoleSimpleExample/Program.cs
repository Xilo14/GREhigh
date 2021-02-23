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
            //Console.ReadLine();

            var api = cluster.GetApiEntryPoint();

            var p1 = api.CreateNewPlayer();
            var p2 = api.CreateNewPlayer();
            var p3 = api.CreateNewPlayer();
            var p4 = api.CreateNewPlayer();
            var p5 = api.CreateNewPlayer();
            api.AddCoins(100, p1);
            Console.WriteLine($"p1: {api.GetAmountCoins(p1)}");
            Console.WriteLine($"p2: {api.GetAmountCoins(p2)}");
            Console.WriteLine($"p3: {api.GetAmountCoins(p3)}");
            Console.WriteLine($"p4: {api.GetAmountCoins(p4)}");
            Console.WriteLine($"p5: {api.GetAmountCoins(p5)}");

            var party = new HoTParty() {
                Players = new List<Player>() { p1 },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            //Console.ReadLine();
            //Console.ReadLine();
            party = new HoTParty() {
                Players = new List<Player>() { p1 },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            while (!cluster.GetPartyProducer().TryProduce(party)) { }
            party = new HoTParty() {
                Players = new List<Player>() { p1 },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            while (!cluster.GetPartyProducer().TryProduce(party)) { }
            Console.ReadLine();
            party = new HoTParty() {
                Players = new List<Player>() { p3, p4 },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            Console.ReadLine();

            party = new HoTParty() {
                Players = new List<Player>() { p5 },
                SearchParams = new HoTRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);

            Console.ReadLine();
            Console.WriteLine($"p1: {api.GetAmountCoins(p1)}");
            Console.WriteLine($"p2: {api.GetAmountCoins(p2)}");
            Console.WriteLine($"p3: {api.GetAmountCoins(p3)}");
            Console.WriteLine($"p4: {api.GetAmountCoins(p4)}");
            Console.WriteLine($"p5: {api.GetAmountCoins(p5)}");
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
            public Transaction Cook(RawTransaction rawTransaction) {
                return rawTransaction;
            }
        }
        public class TransactionChefMoqFactory : IInfrastructureFactory<ITransactionChef> {
            public ITransactionChef GetInfrastructure() {
                return new TransactionChefMoq();
            }
        }
    }
}
