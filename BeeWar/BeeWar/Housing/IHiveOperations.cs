using BeeWar.Bees;
using BeeWar.Products;

namespace BeeWar.Housing
{
    /// <summary>Контракт операций над ульем.</summary>
    public interface IHiveOperations
    {
        void AddBee(Bee bee);
        bool RemoveBee(string beeName);
        double HarvestHoney(double kg);
        Honey ProduceHoney(string sort);
        void Treat();
    }
}