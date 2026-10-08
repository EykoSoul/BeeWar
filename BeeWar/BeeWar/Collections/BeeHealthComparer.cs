using System.Collections.Generic;
using BeeWar.Bees;

namespace BeeWar.Collections
{
    /// <summary>
    /// Компаратор пчёл по здоровью.
    /// IComparer&lt;in T&gt; — контравариантный интерфейс.
    /// </summary>
    public class BeeHealthComparer : IComparer<Bee>
    {
        public int Compare(Bee? x, Bee? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.Health.CompareTo(y.Health);
        }
    }
}