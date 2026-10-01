using System;
using System.Text;
using BeeWar.Bees;
using BeeWar.Management;
using BeeWar.Products;

namespace BeeWar
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("########################################");
            Console.WriteLine("#      ПЧЕЛИНАЯ ФЕРМА — СИМУЛЯЦИЯ      #");
            Console.WriteLine("########################################");

            BeeFactory factory = new StandardBeeFactory();

            var apiary = new Apiary("Луговое разнотравье");
            var beekeeper = new Beekeeper("Иван Петров");
            Console.WriteLine($"\nПчеловод: {beekeeper.Name}");
            Console.WriteLine($"Пасека:   {apiary.Name}");

            apiary.AddHive(factory.CreateFullHive("H1", workers: 12, cells: 480));
            apiary.AddHive(factory.CreateFullHive("H2", workers: 16, cells: 640));
            apiary.AddHive(factory.CreateFullHive("H3", workers: 14, cells: 640));

            Console.WriteLine($"\nСоздано ульев: {apiary.Hives.Count}, пчёл: {apiary.TotalBees()}");
            foreach (var h in apiary.Hives) Console.WriteLine("  " + h.Status());

            Console.WriteLine("\n--- Полиморфизм: каждая пчела работает по-своему ---");
            foreach (var bee in apiary.Hives[0].Bees)
                Console.WriteLine("  " + bee.DoWork());

            Console.WriteLine("\n--- Инкапсуляция: сеттер ограничивает значения ---");
            var testSub = new Substance("Тест", percent: 500, usefulness: 5);
            Console.WriteLine($"  Вещество(процент=500, полезность=5) -> {testSub}");
            var testBee = new WorkerBee("Контроль");
            testBee.Health = -10; Console.WriteLine($"  Здоровье = -10 -> {testBee.Health}");
            testBee.Health = 99999; Console.WriteLine($"  Здоровье = 99999 -> {testBee.Health}");

            var sim = new Simulation(apiary, beekeeper);
            sim.RunSeason(14);

            Console.WriteLine("\n--- Состояние после 14 дней ---");
            foreach (var h in apiary.Hives) Console.WriteLine("  " + h.Status());

            var honey = sim.FinishAndProduce("Луговой мёд");

            Console.WriteLine("\n--- Сравнение сортов мёда ---");
            var forestHoney = new Honey("Лесной", 5.0);
            forestHoney.AddSubstance(new Substance("Падевые вещества", 4.0, 0.7));
            forestHoney.AddSubstance(new Substance("Вода", 22.0, 0.5));
            Console.WriteLine("  " + honey);
            Console.WriteLine("  " + forestHoney);

            Console.WriteLine("\n--- Смешивание двух партий мёда ---");
            var blend = HoneyQuality.Blend(honey, forestHoney, "Купаж");
            HoneyQuality.PrintReport(blend);

            Console.WriteLine("\n=== Программа завершена ===");
        }
    }
}