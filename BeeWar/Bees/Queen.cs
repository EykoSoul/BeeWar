namespace BeeWar.Bees
{
    public class Queen : Bee
    {
        public int EggsPerDay { get; set; }
        public int EggsLaid { get; private set; }

        public Queen(string name, int eggsPerDay = 1500, int ageDays = 0, int health = 100)
            : base(name, ageDays, health)
        {
            EggsPerDay = eggsPerDay;
        }

        public override string DoWork()
        {
            EggsLaid += EggsPerDay;
            Energy = Energy - 5;
            return $"Матка '{Name}' отложила {EggsPerDay} яиц (всего {EggsLaid}).";
        }
    }
}