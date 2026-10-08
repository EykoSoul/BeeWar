using System;
using System.Collections.Generic;
using System.Linq;

namespace BeeWar.Products
{
    // Мёд — продукт, состоящий из веществ. Содержит логику оценки качества
    public class Honey
    {
        private readonly List<Substance> _composition = new();
        private double _massKg;

        public string Sort { get; set; }
        public double MassKg
        {
            get => _massKg;
            set => _massKg = Math.Max(0, value);
        }

        public IReadOnlyList<Substance> Composition => _composition;

        public Honey(string sort, double massKg = 0)
        {
            Sort = sort;
            MassKg = massKg;
            _composition.Add(new Substance("Фруктоза", 38.2, 0.95));
            _composition.Add(new Substance("Глюкоза", 31.3, 0.95));
            _composition.Add(new Substance("Сахароза", 3.5, 0.40));
            _composition.Add(new Substance("Вода", 17.0, 0.50));
            _composition.Add(new Substance("Минералы", 2.0, 1.00));
            _composition.Add(new Substance("Витамины", 1.5, 1.00));
            _composition.Add(new Substance("Прочие", 6.5, 0.60));
        }

        public void AddSubstance(Substance s)
        {
            if (s == null) return;
            _composition.Add(s);
        }

        public double EvaluateQuality()
        {
            if (_composition.Count == 0) return 0;
            double totalPercent = _composition.Sum(s => s.Percent);
            if (totalPercent <= 0) return 0;

            double weighted = _composition.Sum(s => s.Percent * s.Usefulness) / totalPercent;

            double water = _composition.Where(s => s.Name == "Вода").Sum(s => s.Percent);
            double sucrose = _composition.Where(s => s.Name == "Сахароза").Sum(s => s.Percent);
            double penalty = Math.Max(0, water - 18) * 1.5 + Math.Max(0, sucrose - 5) * 2.0;

            double score = weighted * 100 - penalty;
            return Math.Clamp(score, 0, 100);
        }

        public string QualityCategory()
        {
            double q = EvaluateQuality();
            return q switch
            {
                >= 90 => "Превосходное",
                >= 75 => "Отличное",
                >= 60 => "Хорошее",
                >= 40 => "Удовлетворительное",
                _ => "Низкое"
            };
        }

        public override string ToString()
            => $"Мёд '{Sort}': {MassKg:F3} кг, качество {EvaluateQuality():F1} ({QualityCategory()})";
    }
}