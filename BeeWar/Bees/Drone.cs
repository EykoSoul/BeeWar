namespace BeeWar.Bees
{
    public class Drone : Bee
    {
        public bool IsFertile { get; set; } = true;

        public Drone(string name, int ageDays = 0, int health = 100)
            : base(name, ageDays, health) { }

        public override string DoWork()
            => $"Трутень '{Name}' " + (IsFertile ? "готов к спариванию." : "не фертилен.");
    }
}