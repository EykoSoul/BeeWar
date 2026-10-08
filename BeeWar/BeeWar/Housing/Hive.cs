using System;
using System.Collections.Generic;
using System.Linq;
using BeeWar.Bees;
using BeeWar.Products;

namespace BeeWar.Housing
{
    // Улей: содержит пчёл и соты, моделирует день жизни улья
    // Количество пчёл может расти (размножение) и падать (гибель)
    public class Hive : IHiveOperations
    {
        private readonly List<Bee> _bees = new();
        private Honey? _honeyInside;
        private readonly Random _random = new();

        public string Id { get; }
        public Honeycomb Comb { get; }
        public int BeeCount => _bees.Count;
        public IReadOnlyList<Bee> Bees => _bees;

        public int TotalDeaths { get; private set; }
        public int TotalBirths { get; private set; }

        public Hive(string id, int cells = 1000)
        {
            Id = id;
            Comb = new Honeycomb(cells);
        }

        // ==================== IHiveOperations ====================

        public void AddBee(Bee bee) => _bees.Add(bee);

        public bool RemoveBee(string beeName)
        {
            var b = _bees.FirstOrDefault(x => x.Name == beeName);
            return b != null && _bees.Remove(b);
        }

        public double HarvestHoney(double kg) => Comb.Extract(kg);

        public Honey ProduceHoney(string sort)
        {
            double kg = Comb.Extract(Comb.FilledKg);
            _honeyInside = new Honey(sort, kg);
            return _honeyInside;
        }

        public void Treat()
        {
            foreach (var bee in _bees)
                bee.Health = Math.Min(100, bee.Health + 20);
        }

        // ==================== Симуляция ====================

        //  Один день жизни улья. 
        public void SimulateDay(Action<string>? log = null)
        {
            // 1. Работают все живые
            foreach (var bee in _bees.Where(b => b.IsAlive).ToList())
            {
                if (bee is WorkerBee wb)
                {
                    string act = wb.DoWork();
                    double grams = 0.5 + wb.Energy / 200.0;
                    double placed = Comb.AddNectar(grams);
                    log?.Invoke($"  [{Id}] {act} В соты: {placed:F2} г.");
                }
                else
                {
                    log?.Invoke($"  [{Id}] {bee.DoWork()}");
                }
                bee.AgeOneDay();
            }

            // 2. Убираем мёртвых
            int removed = _bees.RemoveAll(b => !b.IsAlive);
            if (removed > 0)
            {
                TotalDeaths += removed;
                log?.Invoke($"  [{Id}] Погибло пчёл: {removed}. Осталось: {_bees.Count}");
            }

            // 3. Замена погибшей матки
            ReplaceDeadQueen(log);

            // 4. Размножение
            BreedNewBees(log);
        }

        // ==================== Приватные методы ====================

        // Если матка погибла, но в улье достаточно рабочих —
        // выводим новую матку
        private void ReplaceDeadQueen(Action<string>? log)
        {
            var aliveQueen = _bees.OfType<Queen>().FirstOrDefault(q => q.IsAlive);
            if (aliveQueen != null) return;

            var workers = _bees.OfType<WorkerBee>().Where(b => b.IsAlive).ToList();
            if (workers.Count < 3) return;

            var newQueen = new Queen($"Матка-{Id}-новая", eggsPerDay: 1200);
            _bees.Add(newQueen);
            TotalBirths++;
            log?.Invoke($"  [{Id}] Выведена новая матка: {newQueen.Name}");
        }

        // Размножение: если матка жива, из накопленных яиц появляются новые пчёлы.
        // Скорость ФИКСИРОВАННАЯ, не зависит от текущей популяции —
        // это защищает от экспоненциального взрыва
        private void BreedNewBees(Action<string>? log)
        {
            var queen = _bees.OfType<Queen>().FirstOrDefault();
            if (queen == null || !queen.IsAlive)
            {
                log?.Invoke($"  [{Id}] [ВНИМАНИЕ] Нет живой матки — размножение остановлено.");
                return;
            }

            // Базовая рождаемость — фиксированная
            int baseHatch = 2;
            if (queen.Health < 50) baseHatch = 1;
            if (queen.Health < 20) baseHatch = 0;

            // Случайность только в плюс: 0 или +1
            int eggsToHatch = baseHatch + _random.Next(0, 2);

            // Верхняя граница
            eggsToHatch = Math.Min(eggsToHatch, 4);

            int available = queen.PendingEggs;
            int hatched = queen.TakeEggs(eggsToHatch);

            log?.Invoke($"  [{Id}] В наличии: {available}, попытка: {eggsToHatch}, вылупилось: {hatched}");

            if (hatched == 0) return;

            for (int i = 0; i < hatched; i++)
            {
                double r = _random.NextDouble();
                Bee newBee;

                bool hasOtherQueen = _bees.OfType<Queen>().Any(q => q.IsAlive && q != queen);

                if (r < 0.05 && !hasOtherQueen)
                    newBee = new Queen($"Матка-{Id}-{_bees.Count + 1}", eggsPerDay: 1000);
                else if (r < 0.25)
                    newBee = new Drone($"Трутень-{Id}-{_bees.Count + 1}");
                else
                    newBee = new WorkerBee($"Рабочая-{Id}-{_bees.Count + 1}");

                _bees.Add(newBee);
                TotalBirths++;
            }

            log?.Invoke($"  [{Id}] Родилось: {hatched}. Всего пчёл: {_bees.Count}");
        }

        // ==================== Прочее ====================

        public string Status()
        {
            var q = _bees.OfType<Queen>().FirstOrDefault(b => b.IsAlive);
            int workers = _bees.OfType<WorkerBee>().Count();
            int drones = _bees.OfType<Drone>().Count();

            return $"Улей {Id}: всего пчёл {BeeCount} " +
                   $"(матка: {(q?.Name ?? "нет")}, рабочих: {workers}, трутней: {drones}) | " +
                   $"родилось: {TotalBirths}, погибло: {TotalDeaths} | {Comb}";
        }
    }
}