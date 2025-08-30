using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarden
{
    public interface IRandomNumberGenerator
    {
        double NextDouble();
        int Next(int min, int max);

    }

    public class DefaultRandomGenerator : IRandomNumberGenerator
    {
        private readonly Random _random = new Random();

        public double NextDouble() 
        {
            return _random.NextDouble();
        } 

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }

    public interface IWeightedItem<T>
    {
        public T Item { get; }
        public int Weight { get; }
    }

    public static class WeightedRandom
    {
        public static TItem ChooseWeightedRandom<TItem, TWeightedItem>(IEnumerable<TWeightedItem> list, IRandomNumberGenerator random) where TWeightedItem : IWeightedItem<TItem>
        {
            int weightsSum = list.Sum(i => i.Weight);
            int randomWeight = random.Next(0, weightsSum);

            foreach (var weightedItem in list)
            {
                if (randomWeight <= weightedItem.Weight)
                {
                    return weightedItem.Item;
                }
                randomWeight -= weightedItem.Weight;
            }

            throw new FailedRandomItemSelectionException("Unable to select weighted random item");
        }
    }
}
