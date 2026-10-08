using System;
using BeeWar.Products;

namespace BeeWar.Management
{
    /// <summary>Пчеловод — управляет пасекой и производит мёд.</summary>
    public class Beekeeper
    {
        public string Name { get; set; }
        public Honey? LastBatch { get; private set; }

        public Beekeeper(string name) { Name = name; }

        public void TreatAll(Apiary apiary)
        {
            foreach (var h in apiary.Hives) h.Treat();
            Console.WriteLine($"  {Name}: все ульи обработаны.");
        }

        public Honey ProduceFromApiary(Apiary apiary, string sort)
        {
            double totalKg = 0;
            foreach (var h in apiary.Hives)
                totalKg += h.HarvestHoney(h.Comb.FilledKg);

            LastBatch = new Honey(sort, totalKg);
            Console.WriteLine($"  {Name}: произведено {totalKg:F3} кг мёда '{sort}'.");
            return LastBatch;
        }
    }
}