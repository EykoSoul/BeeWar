using System;
using System.Collections.Generic;
using BeeWar.Bees;

namespace BeeWar.Collections
{
    // Ковариантность: IEnumerable&lt;out T&gt;
    public static class CovarianceDemo
    {
        public static void PrintAll(IEnumerable<Bee> bees)
        {
            foreach (var bee in bees)
                Console.WriteLine("  " + bee.DoWork());
        }
    }
}