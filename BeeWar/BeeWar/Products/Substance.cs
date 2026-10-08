using System;

namespace BeeWar.Products
{
    /// <summary>Вещество — компонент мёда.</summary>
    public class Substance
    {
        private string _name = "—";
        private double _percent;
        private double _usefulness;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "—" : value;
        }

        public double Percent
        {
            get => _percent;
            set => _percent = Math.Clamp(value, 0, 100);
        }

        public double Usefulness
        {
            get => _usefulness;
            set => _usefulness = Math.Clamp(value, 0, 1);
        }

        public Substance(string name, double percent, double usefulness)
        {
            Name = name;
            Percent = percent;
            Usefulness = usefulness;
        }

        public override string ToString()
            => $"{Name}: {Percent:F1}% (полезность {Usefulness:F2})";
    }
}