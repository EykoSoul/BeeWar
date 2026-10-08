using System;
using System.Collections.Generic;
using BeeWar.Bees;

namespace BeeWar.Collections
{
    /// <summary>
    /// Ковариантность: IEnumerable&lt;out T&gt; позволяет передать
    /// BeeCollection&lt;WorkerBee&gt; туда, где ожидается IEnumerable&lt;Bee&gt;.
    /// </summary>
    public static class CovarianceDemo
    {
        public static void PrintAll(IEnumerable<Bee> bees)
        {
            foreach (var bee in bees)
                Console.WriteLine("  " + bee.DoWork());
        }
    }
}