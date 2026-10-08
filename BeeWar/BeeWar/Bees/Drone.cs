namespace BeeWar.Bees
{
    //  Трутень — участвует в размножении. 
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