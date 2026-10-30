using System;
using BeeWar.Bees;

namespace BeeWar.Collections
{
    /// Набор делегатов сравнения для пчёл. Все правила — внешние по отношению
    /// к BeeCollection: сама коллекция не знает, как сравнивать пчёл
    public static class BeeComparers
    {
        // ===== Func<T, T, int> =====

        //Сравнение по здоровью (по возрастанию).
        public static readonly Func<Bee, Bee, int> ByHealth =
            (a, b) => a.Health.CompareTo(b.Health);

        //Сравнение по возрасту (по возрастанию).
        public static readonly Func<Bee, Bee, int> ByAge =
            (a, b) => a.AgeDays.CompareTo(b.AgeDays);

        //Сравнение по имени (лексикографически).
        public static readonly Func<Bee, Bee, int> ByName =
            (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal);

        //Сравнение по энергии (по убыванию).
        public static readonly Func<Bee, Bee, int> ByEnergyDesc =
            (a, b) => b.Energy.CompareTo(a.Energy);

        // ===== Func<T, TKey> — ключи для SortBy =====

        public static readonly Func<Bee, int> HealthKey = b => b.Health;
        public static readonly Func<Bee, int> AgeKey = b => b.AgeDays;
        public static readonly Func<Bee, string> NameKey = b => b.Name;
        public static readonly Func<Bee, double> EnergyKey = b => b.Energy;

        // ===== Predicate<T> — условия поиска =====

        public static readonly Predicate<Bee> IsAlive = b => b.IsAlive;
        public static readonly Predicate<Bee> IsWeak = b => b.Health < 50;
        public static readonly Predicate<Bee> IsWorker = b => b is WorkerBee;
        public static readonly Predicate<Bee> IsOld = b => b.AgeDays > 30;

        // ===== Action<T> — действия =====

        public static readonly Action<Bee> PrintInfo =
            b => Console.WriteLine("    " + b.GetInfo());

        public static readonly Action<Bee> Heal =
            b => b.Health = Math.Min(100, b.Health + 10);
    }
}