using System;
using System.Collections.Generic;
using System.Linq;
using BeeWar.Bees;
using BeeWar.Products;

namespace BeeWar.Housing
{
    public class Hive : IHiveOperations
    {
        private readonly List<Bee> _bees = new();
        private Honey? _honeyInside;

        public string Id { get; }
        public Honeycomb Comb { get; }
        public int BeeCount => _bees.Count;
        public IReadOnlyList<Bee> Bees => _bees;

        public Hive(string id, int cells = 1000)
        {
            Id = id;
            Comb = new Honeycomb(cells);
        }

        public void AddBee(Bee bee) => _bees.Add(bee);

        public bool RemoveBee(string beeName)
        {
            var b = _bees.FirstOrDefault(x => x.Name == beeName);
            return b != null && _bees.Remove(b);
        }

        public void SimulateDay(Action<string>? log = null)
        {
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

        public string Status()
        {
            var q = _bees.OfType<Queen>().FirstOrDefault();
            return $"Улей {Id}: пчёл {BeeCount}, матка: {(q?.Name ?? "нет")}, {Comb}";
        }
    }
}