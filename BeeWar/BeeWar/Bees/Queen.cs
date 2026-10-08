using System;

namespace BeeWar.Bees
{
    //  Матка — откладывает яйца, из которых появляются новые пчёлы. 
    public class Queen : Bee
    {
        public int EggsPerDay { get; set; }
        public int EggsLaid { get; private set; }

        //  Яйца, готовые к превращению в пчёл. 
        public int PendingEggs { get; private set; }

        public Queen(string name, int eggsPerDay = 1500, int ageDays = 0, int health = 100)
            : base(name, ageDays, health)
        {
            EggsPerDay = eggsPerDay;
            MaxAge = 150;
        }

        //  Матка тратит меньше энергии, чем рабочие. 
        public override void AgeOneDay()
        {
            AgeDays++;
            Energy = Math.Max(0, Energy - 1);
            if (Energy < 10) Health -= 1;
            if (AgeDays >= MaxAge) Health = 0;
        }

        public override string DoWork()
        {
            if (!IsAlive)
                return $"Матка '{Name}' погибла, яйца больше не откладываются.";

            EggsLaid += EggsPerDay;
            PendingEggs += EggsPerDay;
            Energy = Math.Max(0, Energy - 1);

            return $"Матка '{Name}' отложила {EggsPerDay} яиц (всего {EggsLaid}, в ожидании: {PendingEggs}).";
        }

        //  Взять из «очереди» яиц несколько штук для превращения в пчёл. 
        public int TakeEggs(int count)
        {
            int taken = Math.Min(count, PendingEggs);
            PendingEggs -= taken;
            return taken;
        }
    }
}