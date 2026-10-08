using System.Collections.Generic;
using System.Linq;
using BeeWar.Housing;

namespace BeeWar.Management
{
    //  Пасека — набор ульев. 
    public class Apiary
    {
        private readonly List<Hive> _hives = new();

        public string Name { get; }
        public IReadOnlyList<Hive> Hives => _hives;

        public Apiary(string name) { Name = name; }

        public void AddHive(Hive h) => _hives.Add(h);

        public bool RemoveHive(string id)
        {
            var h = _hives.FirstOrDefault(x => x.Id == id);
            return h != null && _hives.Remove(h);
        }

        public int TotalBees() => _hives.Sum(h => h.BeeCount);
        public double TotalFillKg() => _hives.Sum(h => h.Comb.FilledKg);
    }
}