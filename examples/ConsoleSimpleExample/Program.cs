using System;
using System.Collections.Generic;
using System.Linq;
using DataStorageInMemory;
using GREhigh;
using GREhigh.Builders;
using GREhigh.DomainBase;
using GREhigh.Games.HeadsOrTails;
using GREhigh.Games.SingleWinner;
using GREhigh.Games.SoloXStages;
using GREhigh.Games.XRoulette;
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
                    .WithRepository(new RepositoryInMemoryFactory<HoTRoom>())
                    .WithRoomFactory(new HoTRoomFactory())
                    .SaveRoomToRegistry()
                .AddRoomToRegistry<SingleWinnerRoom>()
                    .WithHandler(new SingleWinnerRoomHandlerFactory())
                    .WithRepository(new RepositoryInMemoryFactory<SingleWinnerRoom>())
                    .WithRoomFactory(new SingleWinnerRoomFactory())
                    .SaveRoomToRegistry()
                .AddRoomToRegistry<XRouletteRoom>()
                    .WithHandler(new XRouletteRoomHandlerFactory())
                    .WithRepository(new RepositoryInMemoryFactory<XRouletteRoom>())
                    .WithRoomFactory(new XRouletteRoomFactory())
                    .SaveRoomToRegistry()
                .AddRoomToRegistry<SoloXStagesRoom>()
                    .WithHandler(new SoloXStagesRoomHandlerFactory())
                    .WithRepository(new RepositoryInMemoryFactory<SoloXStagesRoom>())
                    .WithRoomFactory(new SoloXStagesRoomFactory())
                    .SaveRoomToRegistry()
                .Build();

            Room rouletteRoom = null;
            cluster.RoomUpdated += (sender, args) => {
                Console.WriteLine(args.UpdateType.ToString() +
                    $"{args.Room.GetType().Name} {args.Room.RoomId} {args.Room.Status} {args.Room.Players.Count}");

                if (args.Room is XRouletteRoom)
                    rouletteRoom = args.Room;

                // if (args.Room.Status == Room.StatusEnum.Finished) {
                //     Console.WriteLine("w:" + ((SingleWinnerRoomResult)args.Room.Result).winner.PlayerId);
                //     //Console.WriteLine("l:" + ((HoTRoomResult)args.Room.Result).loser.PlayerId);
                // }
            };
            cluster.Start();
            //Console.ReadLine();

            var api = cluster.GetApiEntryPoint();

            var p1 = api.CreateNewPlayer();
            var p2 = api.CreateNewPlayer();
            var p3 = api.CreateNewPlayer();
            var p4 = api.CreateNewPlayer();
            var p5 = api.CreateNewPlayer();
            api.AddCoins(200, p1);
            api.AddCoins(100, p2);
            api.AddCoins(100, p3);
            api.AddCoins(100, p4);
            api.AddCoins(100, p5);
            Console.WriteLine($"p1: {api.GetAmountCoins(p1)}");
            Console.WriteLine($"p2: {api.GetAmountCoins(p2)}");
            Console.WriteLine($"p3: {api.GetAmountCoins(p3)}");
            Console.WriteLine($"p4: {api.GetAmountCoins(p4)}");
            Console.WriteLine($"p5: {api.GetAmountCoins(p5)}");

            var party = new Party<SingleWinnerRoom>() {
                Players = new List<Player>() { p1 },
                SearchParams = new SingleWinnerRoomSearchParams() { Bet = 50 }
            };
            var partyX = new Party<XRouletteRoom>() {
                Players = new List<Player>() { p1 },
            };
            //cluster.GetPartyProducer().TryProduce(party);
            cluster.GetPartyProducer().TryProduce(partyX);
            //Console.ReadLine();
            //Console.ReadLine();
            party = new Party<SingleWinnerRoom>() {
                Players = new List<Player>() { p1 },
                SearchParams = new SingleWinnerRoomSearchParams() { Bet = 50 }
            };
            while (!cluster.GetPartyProducer().TryProduce(party)) { }
            party = new Party<SingleWinnerRoom>() {
                Players = new List<Player>() { p1 },
                SearchParams = new SingleWinnerRoomSearchParams() { Bet = 50 }
            };
            while (!cluster.GetPartyProducer().TryProduce(party)) { }
            //Console.ReadLine();
            party = new Party<SingleWinnerRoom>() {
                Players = new List<Player>() { p3, p4 },
                SearchParams = new SingleWinnerRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            //Console.ReadLine();

            party = new Party<SingleWinnerRoom>() {
                Players = new List<Player>() { p5 },
                SearchParams = new SingleWinnerRoomSearchParams() { Bet = 50 }
            };
            cluster.GetPartyProducer().TryProduce(party);
            while (rouletteRoom == null) { }
            cluster.GetUpdateProducer().TryProduce(new XRouletteUpdateRoom() {
                Bet = new() { Player = p1, Amount = 50, Sector = XRouletteRoom.SectorsEnum.Grey },
                RoomId = rouletteRoom.Id
            });
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
