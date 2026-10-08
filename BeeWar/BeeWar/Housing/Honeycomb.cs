using System;

namespace BeeWar.Housing
{
    /// <summary>Соты — хранят нектар и мёд.</summary>
    public class Honeycomb
    {
        private int _cells;
        private double _filledKg;

        public int Cells
        {
            get => _cells;
            set => _cells = Math.Max(1, value);
        }

        public double FilledKg
        {
            get => _filledKg;
            private set => _filledKg = Math.Clamp(value, 0, CapacityKg);
        }

        public double CapacityKg => Cells * 0.005;
        public bool IsFull => FilledKg >= CapacityKg - 1e-9;
        public double FillPercent => CapacityKg <= 0 ? 0 : FilledKg / CapacityKg * 100;

        public Honeycomb(int cells = 1000)
        {
            Cells = cells;
        }

        public double AddNectar(double grams)
        {
            double kg = grams / 1000.0;
            double free = CapacityKg - FilledKg;
            double placed = Math.Min(kg, free);
            FilledKg += placed;
            return placed * 1000.0;
        }

        public double Extract(double kg)
        {
            double got = Math.Min(kg, FilledKg);
            FilledKg -= got;
            return got;
        }

        public override string ToString()
            => $"Соты: {Cells} ячеек, заполнено {FilledKg:F3}/{CapacityKg:F3} кг ({FillPercent:F1}%)";
    }
}