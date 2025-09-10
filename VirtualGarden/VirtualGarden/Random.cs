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

    //ints are still generated randomly, doubles are picked circularly from an inputed list
    public class FakeRandomGenerator : IRandomNumberGenerator
    {
        private readonly Random _random = new Random();
        private readonly List<double> _generatedDouble;
        private int _doubleIndex = -1;

        public FakeRandomGenerator(List<double> generatedDouble)
        {
            _generatedDouble = generatedDouble;
        }

        public double NextDouble()
        {
            _doubleIndex++;
            if (_doubleIndex == _generatedDouble.Count)
            {
                _doubleIndex = 0;
            }
            return _generatedDouble[_doubleIndex];
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
                if (randomWeight < weightedItem.Weight)
                {
                    return weightedItem.Item;
                }
                randomWeight -= weightedItem.Weight;
            }

            throw new FailedRandomItemSelectionException("Unable to select weighted random item");
        }
    }
}
