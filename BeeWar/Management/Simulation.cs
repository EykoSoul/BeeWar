using System;
using BeeWar.Products;

namespace BeeWar.Management
{
    public class Simulation
    {
        public Apiary Apiary { get; }
        public Beekeeper Beekeeper { get; }
        public int Day { get; private set; }

        public Simulation(Apiary apiary, Beekeeper beekeeper)
        {
            Apiary = apiary;
            Beekeeper = beekeeper;
        }

        public void RunSeason(int days, bool verbose = false)
        {
            Console.WriteLine($"\n=== Симуляция сезона: {days} дней ===");
            for (int d = 1; d <= days; d++)
            {
                Day++;
                if (verbose)
                    Console.WriteLine($"\n--- День {Day} ---");

                foreach (var hive in Apiary.Hives)
                    hive.SimulateDay(verbose ? Console.WriteLine : (Action<string>?)null);
            }
            Console.WriteLine("=== Сезон завершён ===");
        }

        public Honey FinishAndProduce(string sort)
        {
            var honey = Beekeeper.ProduceFromApiary(Apiary, sort);
            Console.WriteLine("\n--- Отчёт о качестве мёда ---");
            HoneyQuality.PrintReport(honey);
            return honey;
        }
    }
}