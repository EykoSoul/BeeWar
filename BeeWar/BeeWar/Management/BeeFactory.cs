using BeeWar.Bees;
using BeeWar.Housing;

namespace BeeWar.Management
{
    // Абстрактная фабрика создания пчёл. Паттерн Abstract Factory
    public abstract class BeeFactory
    {
        public abstract Queen CreateQueen(string name);
        public abstract WorkerBee CreateWorker(string name);
        public abstract Drone CreateDrone(string name);

        //  Создать полноценную семью для улья. 
        public Hive CreateFullHive(string hiveId, int workers = 5, int drones = 2, int cells = 1000)
        {
            var hive = new Hive(hiveId, cells);
            hive.AddBee(CreateQueen("Матка-" + hiveId));
            for (int i = 0; i < workers; i++) hive.AddBee(CreateWorker($"Рабочая-{hiveId}-{i + 1}"));
            for (int i = 0; i < drones; i++) hive.AddBee(CreateDrone($"Трутень-{hiveId}-{i + 1}"));
            return hive;
        }
    }

    //  Стандартная фабрика — обычные пчёлы. 
    public class StandardBeeFactory : BeeFactory
    {
        public override Queen CreateQueen(string name) => new Queen(name);
        public override WorkerBee CreateWorker(string name) => new WorkerBee(name);
        public override Drone CreateDrone(string name) => new Drone(name);
    }
}