using BeeWar.Bees;
using BeeWar.Products;

namespace BeeWar.Housing
{
    public interface IHiveOperations
    {
        void AddBee(Bee bee);
        bool RemoveBee(string beeName);
        double HarvestHoney(double kg);
        Honey ProduceHoney(string sort);
        void Treat();
    }
}