using System;
using System.Collections.Generic;
using BeeWar.Bees;

namespace BeeWar.Collections
{
    public static class ContravarianceDemo
    {
        public static void SortAndPrint<T>(BeeCollection<T> collection, IComparer<T> comparer)
            where T : class
        {
            var array = collection.ToArray();
            Array.Sort(array, comparer);
            foreach (var item in array)
                Console.WriteLine("  " + item);
        }
                
        // Контравариантность: IComparer&lt;Bee&gt;
        public static void Demo()
        {
            IComparer<Bee> beeComparer = new BeeHealthComparer();
            IComparer<Queen> queenComparer = beeComparer;

            var queens = new BeeCollection<Queen>();
            queens.Add(new Queen("Матка-A", health: 60));
            queens.Add(new Queen("Матка-B", health: 95));
            queens.Add(new Queen("Матка-C", health: 80));

            SortAndPrint(queens, queenComparer);
        }
    }
}