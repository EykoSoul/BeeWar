using System;

namespace BeeWar.Bees
{
    /// <summary>
    /// Абстрактная пчела. Демонстрирует наследование и полиморфизм.
    /// </summary>
    public abstract class Bee
    {
        private string _name = "Безымянная";
        private int _health = 100;
        private double _energy = 100;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Безымянная" : value;
        }

        public int Health
        {
            get => _health;
            set => _health = Math.Clamp(value, 0, 100);
        }

        public double Energy
        {
            get => _energy;
            set => _energy = Math.Clamp(value, 0, 100);
        }

        public int AgeDays { get; protected set; }
        public int MaxAge { get; protected set; } = 60;
        public bool IsAlive => Health > 0 && AgeDays < MaxAge;

        protected Bee(string name, int ageDays = 0, int health = 100)
        {
            Name = name;
            AgeDays = ageDays;
            Health = health;
        }

        public abstract string DoWork();

        public virtual void AgeOneDay()
        {
            AgeDays++;
            Energy = Math.Max(0, Energy - 2);
            if (Energy < 10) Health -= 1;
            if (AgeDays >= MaxAge) Health = 0;
        }

        public virtual string GetInfo()
            => $"{GetType().Name} '{Name}' | возраст {AgeDays}/{MaxAge} дн. | HP {Health}% | энергия {Energy:F0}%";

        public override string ToString() => GetInfo();
    }
}