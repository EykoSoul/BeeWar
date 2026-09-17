namespace BeeWar.Bees
{
    public class WorkerBee : Bee
    {
        public double NectarCollected { get; private set; }

        public WorkerBee(string name, int ageDays = 0, int health = 100)
            : base(name, ageDays, health) { }

        public override string DoWork()
        {
            if (!IsAlive) return $"Рабочая '{Name}' погибла.";
            double portion = 0.5 + Energy / 200.0;
            NectarCollected += portion;
            Energy = Energy - 10;
            return $"Рабочая '{Name}' собрала {portion:F2} г нектара (итого {NectarCollected:F2} г).";
        }
    }
}