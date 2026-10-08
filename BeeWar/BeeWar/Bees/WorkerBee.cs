namespace BeeWar.Bees
{
    //  Рабочая пчела — собирает нектар, медленно изнашивается. 
    public class WorkerBee : Bee
    {
        public double NectarCollected { get; private set; }

        public WorkerBee(string name, int ageDays = 0, int health = 100)
            : base(name, ageDays, health)
        {
            MaxAge = 60;   // живёт дольше, чем в прошлой версии
        }

        public override string DoWork()
        {
            if (!IsAlive) return $"Рабочая '{Name}' погибла.";

            double portion = 0.5 + Energy / 200.0;
            NectarCollected += portion;
            Energy = System.Math.Max(0, Energy - 3);   // мягкий износ

            if (Energy < 10) Health -= 1;              // изнашивается только при сильной усталости

            return $"Рабочая '{Name}' собрала {portion:F2} г нектара (итого {NectarCollected:F2} г).";
        }
    }
}