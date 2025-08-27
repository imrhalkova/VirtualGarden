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
    }

    public class DefaultRandomGenerator : IRandomNumberGenerator
    {
        private readonly Random _random = new Random();

        public double NextDouble() 
        {
            return _random.NextDouble();
        } 
    }
}
