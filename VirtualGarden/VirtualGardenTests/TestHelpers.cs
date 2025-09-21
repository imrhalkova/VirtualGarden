using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualGarden;

namespace VirtualGardenTests
{
    internal static class TestHelpers
    {
        internal static bool AreGridsEqual(Tile[,] grid1, Tile[,] grid2)
        {
            if (grid1 is null && grid2 is null)
            {
                return true;
            }
            if (grid1 is null || grid2 is null)
            {
                return false;
            }
            if (grid1.GetLength(0) != grid2.GetLength(0) || grid1.GetLength(1) != grid2.GetLength(1))
            {
                return false;
            }
            for (int i = 0; i < grid1.GetLength(0); i++)
            {
                for (int j = 0; j < grid1.GetLength(1); j++)
                {
                    if (!grid1[i, j].IsEqual(grid2[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    //A flower for testing that grows and blooms for one day and has to be watered every day
    internal class TestFlower1 : FlowerType
    {
        public override List<BugsWeight<Bugs>> BugWeights { get; } = new List<BugsWeight<Bugs>>
        {
            new BugsWeight<Bugs>(BugTypes.CommonBugs, 10),
            new BugsWeight<Bugs>(BugTypes.GardenBugs, 0),
            new BugsWeight<Bugs>(BugTypes.ExoticBugs, 0),
            new BugsWeight<Bugs>(BugTypes.TropicalBugs, 0),
        };
        public TestFlower1() : base("Test Flower", 0, 0, 1, 1, 5, "") { }
    }
}
