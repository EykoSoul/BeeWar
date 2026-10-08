using System;
using System.Linq;
using BeeWar.Products;

namespace BeeWar.Management
{
    //  Симуляция сезона пчеловодства. 
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
                {
                    if (verbose)
                    {
                        // Полный лог каждого действия
                        hive.SimulateDay(Console.WriteLine);
                    }
                    else
                    {
                        // Краткий лог: только важные события (рождение/смерть/замена матки)
                        hive.SimulateDay(msg =>
                        {
                            if (msg.Contains("[РОЖДЕНИЕ]") ||
                                msg.Contains("[ГИБЕЛЬ]") ||
                                msg.Contains("[МАТКА]") ||
                                msg.Contains("[ВНИМАНИЕ]"))
                            {
                                Console.WriteLine(msg);
                            }
                        });
                    }
                }

                if (Day % 5 == 0)
                {
                    Console.WriteLine($"\n[День {Day}] Популяция:");
                    foreach (var h in Apiary.Hives)
                        Console.WriteLine($"  {h.Id}: пчёл {h.BeeCount}, " +
                                          $"родилось {h.TotalBirths}, погибло {h.TotalDeaths}");
                }
            }

            Console.WriteLine("\n=== Сезон завершён ===");
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