using System;
using System.Linq;
using System.Text;
using BeeWar.Bees;
using BeeWar.Collections;
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

            // Все ульи — с одинаковыми стартовыми условиями
            apiary.AddHive(factory.CreateFullHive("H1", workers: 5, cells: 800));
            apiary.AddHive(factory.CreateFullHive("H2", workers: 5, cells: 800));
            apiary.AddHive(factory.CreateFullHive("H3", workers: 5, cells: 800));

            Console.WriteLine($"\nСоздано ульев: {apiary.Hives.Count()}, пчёл: {apiary.TotalBees()}");
            foreach (var h in apiary.Hives) Console.WriteLine("  " + h.Status());

            // ============ ПОЛИМОРФИЗМ ДЛЯ КАЖДОГО УЛЬЯ ============
            Console.WriteLine("\n--- Полиморфизм: каждая пчела работает по-своему ---");
            foreach (var hive in apiary.Hives)
            {
                Console.WriteLine($"\n  === Улей {hive.Id} ===");
                foreach (var bee in hive.Bees)
                    Console.WriteLine("  " + bee.DoWork());
            }

            // ============ ИНКАПСУЛЯЦИЯ ============
            Console.WriteLine("\n--- Инкапсуляция: сеттер ограничивает значения ---");
            var testSub = new Substance("Тест", percent: 500, usefulness: 5);
            Console.WriteLine($"  Вещество(процент=500, полезность=5) -> {testSub}");
            var testBee = new WorkerBee("Контроль");
            testBee.Health = -10; Console.WriteLine($"  Здоровье = -10 -> {testBee.Health}");
            testBee.Health = 99999; Console.WriteLine($"  Здоровье = 99999 -> {testBee.Health}");

            // ============ СИМУЛЯЦИЯ ============
            Console.WriteLine("\n\n=========== СИМУЛЯЦИЯ 60 ДНЕЙ ===========");
            var sim = new Simulation(apiary, beekeeper);
            sim.RunSeason(60);

            Console.WriteLine("\n--- Итоговое состояние ульев ---");
            foreach (var h in apiary.Hives) Console.WriteLine("  " + h.Status());

            int totalBirths = apiary.Hives.Sum(h => h.TotalBirths);
            int totalDeaths = apiary.Hives.Sum(h => h.TotalDeaths);
            Console.WriteLine($"\nВсего родилось: {totalBirths}, погибло: {totalDeaths}");
            Console.WriteLine($"Итоговая популяция: {apiary.TotalBees()} пчёл");

            // Производство мёда
            var honey = sim.FinishAndProduce("Луговой мёд");

            // Сравнение сортов
            Console.WriteLine("\n--- Сравнение сортов мёда ---");
            var forestHoney = new Honey("Лесной", 5.0);
            forestHoney.AddSubstance(new Substance("Падевые вещества", 4.0, 0.7));
            forestHoney.AddSubstance(new Substance("Вода", 22.0, 0.5));
            Console.WriteLine("  " + honey);
            Console.WriteLine("  " + forestHoney);

            // Смешивание
            Console.WriteLine("\n--- Смешивание двух партий мёда ---");
            var blend = HoneyQuality.Blend(honey, forestHoney, "Купаж");
            HoneyQuality.PrintReport(blend);

            // ============ ДЕМОНСТРАЦИЯ ОБОБЩЁННОЙ КОЛЛЕКЦИИ ============
            Console.WriteLine("\n\n========= ДЕМОНСТРАЦИЯ ОБОБЩЁННОЙ КОЛЛЕКЦИИ =========");

            // 1. Создание и наполнение BeeCollection<WorkerBee>
            var workers = new BeeCollection<WorkerBee>();
            workers.Add(new WorkerBee("Пчела-1", health: 80));
            workers.Add(new WorkerBee("Пчела-2", health: 90));
            workers.Add(new WorkerBee("Пчела-3", health: 70));
            Console.WriteLine($"\n1. BeeCollection<WorkerBee>: {workers.Count} элементов");
            foreach (var w in workers)
                Console.WriteLine("   " + w.GetInfo());

            // 2. Индексатор и Contains
            Console.WriteLine($"\n2. workers[1] = {workers[1].Name}");
            Console.WriteLine($"   Contains(workers[0]) = {workers.Contains(workers[0])}");

            // 3. ICloneable — клонирование
            var workersClone = (BeeCollection<WorkerBee>)workers.Clone();
            workersClone.Add(new WorkerBee("Новая"));
            Console.WriteLine($"\n3. Оригинал: {workers.Count}, клон: {workersClone.Count}");

            // 4. Ковариантность: BeeCollection<WorkerBee> → IEnumerable<Bee>
            Console.WriteLine("\n4. Ковариантность (BeeCollection<WorkerBee> → IEnumerable<Bee>):");
            CovarianceDemo.PrintAll(workers);

            // 5. Контравариантность: IComparer<Bee> → IComparer<Queen>
            Console.WriteLine("\n5. Контравариантность (IComparer<Bee> → IComparer<Queen>):");
            ContravarianceDemo.Demo();

            // 6. Работа коллекции внутри улья
            Console.WriteLine($"\n6. Улей H1 использует BeeCollection<Bee>: {apiary.Hives.First().BeeCount} пчёл");

            // 7. Ограничение generic-параметра
            Console.WriteLine("\n7. Ограничение where T : class:");
            Console.WriteLine("   BeeCollection<WorkerBee>   OK (ссылочный тип)");
            Console.WriteLine("   BeeCollection<int>         НЕ компилируется (int — не class)");

            // 8. RemoveAll — собственный метод коллекции
            int removed = workers.RemoveAll(w => w.Health < 75);
            Console.WriteLine($"\n8. RemoveAll(Health < 75): удалено {removed}, осталось {workers.Count}");

            Console.WriteLine("\n=== Программа завершена ===");
        }
    }
}