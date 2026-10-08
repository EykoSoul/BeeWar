namespace BeeWar.Bees
{
    /// <summary>Трутень — участвует в размножении.</summary>
    public class Drone : Bee
    {
        public bool IsFertile { get; set; } = true;

        public Drone(string name, int ageDays = 0, int health = 100)
            : base(name, ageDays, health)
        {
            MaxAge = 40;
        }

        public override string DoWork()
            => $"Трутень '{Name}' " + (IsFertile ? "готов к спариванию." : "не фертилен.");
    }
}