using System;
using System.Linq;

namespace BeeWar.Products
{
    //  Анализ и смешивание партий мёда. 
    public static class HoneyQuality
    {
        public static void PrintReport(Honey honey)
        {
            Console.WriteLine($"  Состав мёда '{honey.Sort}':");
            foreach (var s in honey.Composition)
                Console.WriteLine("    - " + s);
            Console.WriteLine($"  Итоговое качество: {honey.EvaluateQuality():F1}/100 " +
                              $"({honey.QualityCategory()})");
        }

        public static Honey Blend(Honey a, Honey b, string newSort)
        {
            double mass = a.MassKg + b.MassKg;
            var result = new Honey(newSort, mass);
            var names = a.Composition.Select(s => s.Name)
                .Concat(b.Composition.Select(s => s.Name)).Distinct();
            foreach (var name in names)
            {
                double p = a.Composition.FirstOrDefault(x => x.Name == name)?.Percent ?? 0;
                double u1 = a.Composition.FirstOrDefault(x => x.Name == name)?.Usefulness ?? 0;
                double q = b.Composition.FirstOrDefault(x => x.Name == name)?.Percent ?? 0;
                double u2 = b.Composition.FirstOrDefault(x => x.Name == name)?.Usefulness ?? 0;
                result.AddSubstance(new Substance(name, (p + q) / 2.0, (u1 + u2) / 2.0));
            }
            return result;
        }
    }
}