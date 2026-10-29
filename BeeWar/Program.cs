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

            // ============ СОРТИРОВКА И СРАВНЕНИЕ ЧЕРЕЗ ДЕЛЕГАТЫ ============
            Console.WriteLine("\n========= СОРТИРОВКА И СРАВНЕНИЕ ЧЕРЕЗ ДЕЛЕГАТЫ =========");

            // Готовим тестовый набор пчёл
            var testBees = new BeeCollection<Bee>();
            testBees.Add(new WorkerBee("Рабочая-А", health: 70, ageDays: 10));
            testBees.Add(new WorkerBee("Рабочая-Б", health: 95, ageDays: 25));
            testBees.Add(new WorkerBee("Рабочая-В", health: 40, ageDays: 5));
            testBees.Add(new Queen("Матка-1", health: 88, ageDays: 100));
            testBees.Add(new Drone("Трутень-1", health: 60, ageDays: 15));

            Console.WriteLine("\nИсходный набор:");
            testBees.ForEach(BeeComparers.PrintInfo);

            // 1. Сортировка по здоровью — Func<Bee, Bee, int>
            Console.WriteLine("\n1. Сортировка по здоровью (Func<Bee,Bee,int>):");
            testBees.Sort(BeeComparers.ByHealth);
            testBees.ForEach(BeeComparers.PrintInfo);

            // 2. Сортировка по имени — другой делегат
            Console.WriteLine("\n2. Сортировка по имени:");
            testBees.Sort(BeeComparers.ByName);
            testBees.ForEach(BeeComparers.PrintInfo);

            // 3. Сортировка по ключу — Func<Bee, TKey>
            Console.WriteLine("\n3. Сортировка по возрасту (SortBy, Func<Bee,int>):");
            testBees.SortBy(BeeComparers.AgeKey);
            testBees.ForEach(BeeComparers.PrintInfo);

            // 4. Сортировка с лямбдой прямо в вызове
            Console.WriteLine("\n4. Сортировка по энергии (по убыванию, лямбда):");
            testBees.Sort((a, b) => b.Energy.CompareTo(a.Energy));
            testBees.ForEach(BeeComparers.PrintInfo);

            // 5. Сравнение двух элементов через Compare
            Console.WriteLine("\n5. Сравнение двух элементов (Compare):");
            var b1 = testBees[0];
            var b2 = testBees[1];
            int cmp = testBees.Compare(b1, b2, BeeComparers.ByHealth);
            Console.WriteLine($"   {b1.Name} vs {b2.Name} по здоровью: {cmp} " +
                              $"({(cmp < 0 ? "меньше" : cmp > 0 ? "больше" : "равно")})");

            // 6. Поиск через Predicate<T>
            Console.WriteLine("\n6. Find / FindAll (Predicate<Bee>):");
            var weak = testBees.Find(BeeComparers.IsWeak);
            Console.WriteLine($"   Первая слабая пчела: {weak?.Name ?? "нет"}");
            var allWeak = testBees.FindAll(BeeComparers.IsWeak);
            Console.WriteLine($"   Всего слабых пчёл: {allWeak.Count}");

            // 7. Действие через Action<T>
            Console.WriteLine("\n7. Лечим всех через ForEach (Action<Bee>):");
            testBees.ForEach(BeeComparers.Heal);
            testBees.ForEach(BeeComparers.PrintInfo);

            // 8. MinBy / MaxBy — Func<Bee, TKey>
            Console.WriteLine("\n8. MinBy / MaxBy по здоровью:");
            var minHp = testBees.MinBy(BeeComparers.HealthKey);
            var maxHp = testBees.MaxBy(BeeComparers.HealthKey);
            Console.WriteLine($"   Min HP: {minHp?.Name} ({minHp?.Health}%)");
            Console.WriteLine($"   Max HP: {maxHp?.Name} ({maxHp?.Health}%)");

            // 9. Сортировка через IComparer<T>
            Console.WriteLine("\n9. Сортировка через IComparer<Bee>:");
            testBees.Sort(new BeeHealthComparer());
            testBees.ForEach(BeeComparers.PrintInfo);

            Console.WriteLine("\n=== Программа завершена ===");
        }
    }
}